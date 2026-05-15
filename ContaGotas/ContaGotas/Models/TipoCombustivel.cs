using Newtonsoft.Json;

namespace ContaGotas;


public class TipoCombustivel :IValido
{ 
    [JsonProperty("Id", Required = Required.Always)]
    public int  Id { get; set; } //required para obrigar a preencer com valor
    
    [JsonProperty("Descritivo")]
    public string Nome { get; set; }
    
    public bool IsValido()
    {
        //Validação ID
        if (Id < 0 )
            return false;
        //Validação do Nome
        if (string.IsNullOrWhiteSpace(Nome))
            return false;

        return true;
    }
}