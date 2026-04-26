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
    public async Task<T?> ObterDadosDgegAsync<T>(string urlDestino)
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

                string textoJson = await resposta.Content.ReadAsStringAsync();

                JsonSerializerOptions opcoes = new JsonSerializerOptions();
                opcoes.PropertyNameCaseInsensitive = true;

                // 1. Lemos o documento JSON inteiro de forma dinâmica
                JsonDocument documento = JsonDocument.Parse(textoJson);
            
                // 2. Vamos diretos à "gaveta" chamada "resultado" (que é onde está a lista no JSON deles)
                JsonElement gavetaResultado = documento.RootElement.GetProperty("resultado");

                // 3. Convertemos APENAS essa gaveta para a nossa lista de postos
                return JsonSerializer.Deserialize<T>(gavetaResultado.GetRawText(), opcoes);


            }
            // quando estas sem internet ou URL invalido
            catch (HttpRequestException  e) when (e.Message.Contains("443"))
            {
                    
                    
                Console.WriteLine(e.Message+"\n");
                if (tentativas == maxTentativas)
                {
                    continuar = false;
                    Console.WriteLine("Servidor não encontrado");
                }
                
            }
        }
        return default;
    }
    

    
    public async Task<List<PrecoMedio>> ObterMediasAsync()
    {
        await Task.Delay(500);
        return new List<PrecoMedio>
        {
            new PrecoMedio(1.55, "Gasolina", "Lisboa"),
            new PrecoMedio(1.62, "Gasóleo", "Porto"),
            new PrecoMedio(1.48, "Gasolina", "Coimbra")
        };
    }
    
}





public class PrecoMedio
{
    public double Valor { get; set; }
    public string Combustivel { get; set; }
    public string Distrito { get; set; }

    public PrecoMedio(double valor, string combustivel, string distrito)
    {
        Valor = valor;
        Combustivel = combustivel;
        Distrito = distrito;
    }
}


