using System.Text.Json;

namespace ContaGotas;


public interface ICombustivelService
{
    Task<List<PrecoMedio>> ObterMediasAsync();
    
}

// Implementação real que lida com HTTP
public class CombustivelApiService : ICombustivelService
{
    private static readonly HttpClient _client = new HttpClient();
    
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

                JsonSerializerOptions opcoes = new JsonSerializerOptions();
                opcoes.PropertyNameCaseInsensitive = true;

                JsonDocument response = JsonDocument.Parse(jsonResponse);

                // CORREÇÃO DO BUG: Validar a existência das propriedades de forma segura (sem estourar)
                // Se a DGEG estiver em manutenção e devolver um erro em HTML em vez de JSON, ou se mudarem o nome da variável, 
                // este GetProperty vai disparar uma KeyNotFoundException
                if (!response.RootElement.TryGetProperty("status", out JsonElement statusElement) || !statusElement.GetBoolean()) 
                {
                   return default;
                } 
                
                if (!response.RootElement.TryGetProperty("resultado", out JsonElement resultado)) 
                {
                   return default;
                }

                return JsonSerializer.Deserialize<T>(resultado.GetRawText(), opcoes);
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
                await Task.Delay(2000); 
            }
            // Qualquer outro erro (ex: o JSON vinha estragado)
            catch (Exception e)
            {
                // Qualquer erro inesperado é capturado aqui para não crashar a app
                throw new Exception($"Erro inesperado ao ler dados da DGEG: {e.Message}");
            }
        }
        return default;
    }
    
    public async Task<List<PrecoMedio>> ObterMediasAsync()
    {
        String paramTipoCombustíveis = "idsTiposComb=1120,3400,3205,3405,3201,2105,2101";
        // No futuro isto talvez deve de ser dinâmico
        String paramData = "dataIni=2026/03/01"; 
        
        String url = baseUrl+"PrecoComb/PMD?"+ paramTipoCombustíveis +"&&" + paramData;
        List<PrecoMedio> listaMedias = await chamarDGEG<List<PrecoMedio>>(url);
        
        // CORREÇÃO: O código morto foi todo apagado. Só fica isto!
        // Já não precisam de dados simulados
        return listaMedias ?? new List<PrecoMedio>();
    }
    
}



public class PrecoMedio
{
    [System.Text.Json.Serialization.JsonPropertyName("PrecoMedio")]
    public string valor { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("TipoCombustivel")]
    public string combustivel { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("Distrito")]
    public string distrito { get; set; }

    public PrecoMedio(string valor, string combustivel, string distrito)
    {
        this.valor = valor;
        this.combustivel = combustivel;
        this.distrito = distrito;
    }
}


