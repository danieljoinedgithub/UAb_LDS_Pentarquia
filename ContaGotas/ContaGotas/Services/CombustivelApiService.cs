using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContaGotas;


public class PrecoDgegConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetDecimal();
        
        string value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value)) return 0;

        // Remove "€"
        string cleaned = value.Replace("€", "").Trim();
        return decimal.Parse(cleaned , new System.Globalization.CultureInfo("pt-PT"));
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}


public interface ICombustivelService
{
    Task<List<PrecoMedioModel>> ObterMediasAsync(int diasAntes = -7);
    
}

// Implementação real que lida com HTTP
public class CombustivelApiService : ICombustivelService
{
    private HttpClient _client = new HttpClient();
    
    private static readonly String baseUrl = "https://precoscombustiveis.dgeg.gov.pt/api/";
    //Contrutor

    public CombustivelApiService()
    {
        if (!_client.DefaultRequestHeaders.Contains("User-Agent"))
        {
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
        }
    }

    public void setClient(HttpClient cliente)
    {
        this._client = cliente;
    }
    
    // chamada e processo da resposta da API DGEG
    public async Task<T?> chamarDGEG<T>(string urlDestino)
    {
        int tentativas = 0;
        const int maxTentativas = 3;

        // Simplificámos o while!
        while (tentativas < maxTentativas)
        {
            try
            {
                tentativas++;
                HttpResponseMessage resposta = await _client.GetAsync(urlDestino);
                resposta.EnsureSuccessStatusCode();

                string jsonResponse = await resposta.Content.ReadAsStringAsync();

                //Validação de resposta vazia
                if (string.IsNullOrWhiteSpace(jsonResponse))
                    return default;

                JsonSerializerOptions opcoes = new JsonSerializerOptions();
                opcoes.PropertyNameCaseInsensitive = true;
                opcoes.Converters.Add(new PrecoDgegConverter());

                JsonDocument response = JsonDocument.Parse(jsonResponse);
                
                // Se a DGEG estiver em manutenção e devolver um erro em HTML em vez de JSON, ou se mudarem o nome da variável, 
                // este GetProperty vai disparar uma KeyNotFoundException
                if (!response.RootElement.TryGetProperty("status", out JsonElement statusElement) ||
                    !statusElement.GetBoolean())
                {
                    return default;
                }

                if (!response.RootElement.TryGetProperty("resultado", out JsonElement resultado))
                {
                    return default;
                }
                
                // caso não tiver 
                /*if (resultado.GetRawText() == "[]" )
                {
                    //por agora o catch nesta classe apanha
                    throw new Exception("Resultado DGEG Vazio: ");
                }*/

                var dados = JsonSerializer.Deserialize<T>(resultado.GetRawText(), opcoes);
                return dados;
            }
            // Falha de Internet ou Servidor da DGEG
            catch (HttpRequestException e)
            {
                if (tentativas == maxTentativas)
                {
                    // Lança o erro para cima, o Controller try-catch vai apanhar!
                    throw new Exception("Servidor da DGEG não encontrado após várias tentativas.");
                }

                // CORREÇÃO DO BUG: Esperar 2 segundos antes de tentar de novo
                // Ia fazer 3 pedidos à DGEG numa fração de segundo, o que não dá tempo para a internet voltar.
                await Task.Delay(2000);}
            catch (JsonException e)
            {
                if (tentativas == maxTentativas)
                {
                    throw new Exception($"Algum campo critico no Json esta null ou em falta: {e.Message}.");
                }
                
                await Task.Delay(2000);
            }
            //quando algum campo com variavel defenida como required apanhar um null o conversor manda FormatException
            catch (System.FormatException e)
            {
                if (tentativas == maxTentativas)
                {
                    throw new Exception($"Algum campo critico no Json esta null ou em falta: {e.Message}.");
                }
                
                await Task.Delay(2000);
            }
            // Validação do timeout da chamada
            catch (TaskCanceledException e)
            {
                if (tentativas == maxTentativas)
                {
                    throw new Exception("Timeout ao chamar a API da DGEG.");
                }
            }
            // Validação dos dados JSON recebidos
            catch (Exception e)
            {
                // Qualquer erro inesperado é capturado aqui para não crashar a app
                throw new Exception($"Erro inesperado ao ler dados da DGEG: {e.Message}");
            }
        }
        return default;
    }
    
    public async Task<List<PrecoMedioModel>> ObterMediasAsync(int diasAntes = -7)
    {
        String paramTipoCombustíveis = "idsTiposComb=1120,3400,3205,3405,3201,2105,2101";

        // Obter as médias com a data dinâmica
        if (diasAntes > 0)
            diasAntes = -diasAntes;
        string paramData = $"dataIni={DateTime.Now.AddDays(diasAntes):yyyy/MM/dd}";
        
        String url = baseUrl+"PrecoComb/PMD?"+ paramTipoCombustíveis +"&" + paramData;
        List<PrecoMedioModel> listaMedias = await chamarDGEG<List<PrecoMedioModel>>(url);

        if (listaMedias == null)
            return new List<PrecoMedioModel>();

        //Validação de dados para garantir que só são devolvidos dados válidos
        return listaMedias
            .Where(m => m.IsValido())
            .ToList();
    }
    
}

