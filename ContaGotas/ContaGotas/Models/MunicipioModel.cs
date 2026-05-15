using Newtonsoft.Json;

namespace ContaGotas;

public class MunicipioModel : IValido
{ 
    [JsonProperty("ID")]
    public int Id { get; set; } 
    
    [JsonProperty("Descricao")]
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