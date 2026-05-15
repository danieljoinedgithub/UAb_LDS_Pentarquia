using Newtonsoft.Json;

namespace ContaGotas;

public class PostoModel : IValido
{ 
    [JsonProperty("Id", Required = Required.Always)]
    public int  Id { get; set; } //required para obrigar a preencer com valor
    
    [JsonProperty("Nome")]
    public string Nome { get; set; } 
    
    [JsonProperty("Morada")]
    public string Morada { get; set; }
    
    [JsonProperty("Preco", Required = Required.Always)]
    public decimal PrecoString { get; set; } // A API às vezes envia como string "1,749 €"
    
    public bool IsValido()
    {
        //Validação ID
        if (Id < 0 )
            return false;
        //Validação do Nome
        if (string.IsNullOrWhiteSpace(Nome))
            return false;
        if (string.IsNullOrWhiteSpace(Morada))
            return false;
        if (PrecoString <= 0)
            return false;

        return true;
    }

    // Propriedade auxiliar para cálculos
    //public double Preco => double.TryParse(PrecoString?.Split(' ')[0].Replace(',', '.'), out var res) ? res : 0;
}