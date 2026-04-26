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
        //tentativas de connecção
        int tentativas = 0;
        const int maxTentativas = 3;
        bool continuar = true;

        while (continuar)
        {
            try
            {
                tentativas++;
                HttpResponseMessage resposta = await _client.GetAsync(urlDestino);
                resposta.EnsureSuccessStatusCode(); 

                string jsonResponse = await resposta.Content.ReadAsStringAsync();

                JsonSerializerOptions opcoes = new JsonSerializerOptions();
                opcoes.PropertyNameCaseInsensitive = true;

                // 1. Lemos o documento JSON inteiro de forma dinâmica
                JsonDocument response = JsonDocument.Parse(jsonResponse);
                //Console.WriteLine("JSON RECEBIDO: " + jsonResponse);

                //TODO validar se a propriedade status existe
                //if (documento.RootElement.TryGetProperty("status", out JsonElement statusElement))

                bool status = response.RootElement.GetProperty("status").GetBoolean();
                if (!status) {
                    //Devolve valor padrão - null para a lista
                   return default;
                } 
                
                //TODO validar se a propriedade resultado existe
                
                // 2. Vamos diretos à "gaveta" chamada "resultado" (que é onde está a lista no JSON deles)
                JsonElement resultado = response.RootElement.GetProperty("resultado");

                // 3. Retorna as propriedades JSON que satisfaça a igualdade do tipo de dados introduzido
                return JsonSerializer.Deserialize<T>(resultado.GetRawText(), opcoes);
            }
            // Exception Quando estas sem internet ou URL invalido
            catch (HttpRequestException  e)
            {
                if (tentativas == maxTentativas)
                {
                    Console.WriteLine("Servidor não encontrado");
                    throw;  
                }
                
            }
        }return default;
    }
    

    
    public async Task<List<PrecoMedio>> ObterMediasAsync()
    {
        String paramTipoCombustíveis = "idsTiposComb=1120,3400,3205,3405,3201,2105,2101";
        String paramData = "dataIni=2026/03/01";
        String url = baseUrl+"PrecoComb/PMD?"+ paramTipoCombustíveis +"&&" + paramData;
        List<PrecoMedio> listaMedias = await chamarDGEG<List<PrecoMedio>>(url);
        return listaMedias ?? new List<PrecoMedio>();
        
        await Task.Delay(500);
        return new List<PrecoMedio>
        {
            new PrecoMedio("1.55 €", "Gasolina", "Lisboa"),
            new PrecoMedio("1.62 €", "Gasóleo", "Porto"),
            new PrecoMedio("1.48 €", "Gasolina", "Coimbra")
        };
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


