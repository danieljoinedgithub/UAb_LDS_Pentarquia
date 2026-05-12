using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContaGotas;

public class PrecoDgegConverter : JsonConverter<decimal>
{
    public override decimal ReadJson(JsonReader reader, Type objectType, decimal existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer)
            return Convert.ToDecimal(reader.Value);
        
        string value = reader.Value?.ToString();
        if (string.IsNullOrWhiteSpace(value)) return 0;

        // Remove "€"
        string cleaned = value.Replace("€", "").Trim();
        return decimal.Parse(cleaned , new System.Globalization.CultureInfo("pt-PT"));
    }
    
    public override void WriteJson(JsonWriter writer, decimal value, JsonSerializer serializer)
    {
        writer.WriteValue(value);
    }
}


public interface ICombustivelService
{
    Task<List<PrecoMedioModel>> ObterMediasAsync(int diasAntes = -7, bool incluirDiferenca = false);
    Task<List<TipoCombustivel>> ObterTiposAsync();
    Task<List<Distrito>> ObterDistritosAsync();
    Task<List<Posto>> ObterPostosAsync(int tipo, int distrito);

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

                JObject response = JObject.Parse(jsonResponse);
                
                // Se a DGEG estiver em manutenção e devolver um erro em HTML em vez de JSON, ou se mudarem o nome da variável, 
                // este GetProperty vai disparar uma KeyNotFoundException
                bool status = response["status"]?.Value<bool>() ?? false;
                if (!status) return default;
                
                JToken resultado = response["resultado"];
                
                //por agora o catch nesta classe apanha
                if (resultado == null ||!resultado.HasValues)
                    throw new Exception("Resultados:[]");
                
                JsonSerializerSettings opcoes = new JsonSerializerSettings();
                opcoes.Converters.Add(new PrecoDgegConverter());
                JsonSerializer serializer = JsonSerializer.Create(opcoes);
                
                
                var dados = resultado.ToObject<T>(serializer);
                return dados;
            }
            
            //=== EXCEÇÕES ====//
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
            catch (JsonReaderException e)
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
                await Task.Delay(2000);
            }
            // Validação dos dados JSON recebidos
            catch (Exception e)
            {

                if (e.Message == "Resultados:[]")
                {
                    throw new Exception("Resultados:[]");
                }
                
                
                // Qualquer erro inesperado é capturado aqui para não crashar a app
                throw new Exception($"Erro inesperado ao ler dados da DGEG: {e.Message}");
            }
        }
        return default;
    }
    
    public async Task<List<PrecoMedioModel>> ObterMediasAsync(int diasAntes = -7, bool incluirDiferenca = false)
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
        var mediasAtuais = listaMedias
            .Where(m => m.IsValido())
            .ToList();

        if (incluirDiferenca)
        {
            //Obter as medias anteriores
            var mediasAntigas = await ObterMediasAsync(diasAntes * 2);

            foreach (var atual in mediasAtuais)
            {
                var antiga = mediasAntigas
                    .FirstOrDefault(a => a.combustivel == atual.combustivel);

                if (antiga != null)
                {
                    atual.valorAnterior = antiga.GetPrecoDecimal();
                }
            }
        }

        return mediasAtuais;
    }

    public async Task<List<TipoCombustivel>> ObterTiposAsync()
    {
        String url = baseUrl + "PrecoComb/GetTiposCombustiveis";
        List<TipoCombustivel> listaTipos = await chamarDGEG<List<TipoCombustivel>>(url);
        return listaTipos;
    }
    public async Task<List<Distrito>> ObterDistritosAsync()
    {
        String url = baseUrl + "PrecoComb/GetDistritos";
        List<Distrito> listaDistritos = await chamarDGEG<List<Distrito>>(url);
        return listaDistritos;
    }

    public async Task<List<Posto>> ObterPostosAsync(int posto, int distrito)
    {
        String url = baseUrl + "PrecoComb/PesquisarPostos?idsTiposComb="+posto+"&idDistrito="+distrito;
        List<Posto> listaDistritos = await chamarDGEG<List<Posto>>(url);
        return listaDistritos;
    }
}

