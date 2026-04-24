namespace ContaGotas;

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

public interface ICombustivelService
{
    Task<List<PrecoMedio>> ObterMediasAsync();
}



// Implementação real que lida com HTTP
public class CombustivelApiService : ICombustivelService
{
    private readonly HttpClient _client = new HttpClient();

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