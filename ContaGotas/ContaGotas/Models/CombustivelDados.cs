using Newtonsoft.Json;

namespace ContaGotas;


public class TipoCombustivel 
{ 
    [JsonProperty("Id", Required = Required.Always)]
    public int  Id { get; set; } //required para obrigar a preencer com valor
    
    [JsonProperty("Descritivo")]
    public string Nome { get; set; } 
}


public class Marca 
{ 
    [JsonProperty("ID")]
    public int Id { get; set; } 
    
    [JsonProperty("Descricao")]
    public string Nome { get; set; } 
}


public class Distrito 
{ 
    [JsonProperty("ID")]
    public int Id { get; set; } 
    
    [JsonProperty("Descritivo")]
    public string Nome { get; set; } 
}


public class Municipio 
{ 
    [JsonProperty("ID")]
    public int Id { get; set; } 
    
    [JsonProperty("Descricao")]
    public string Nome { get; set; } 
}


public class Posto 
{ 
    [JsonProperty("Nome")]
    public string Nome { get; set; } 
    
    [JsonProperty("Morada")]
    public string Morada { get; set; }
    
    [JsonProperty("Preco", Required = Required.Always)]
    public decimal PrecoString { get; set; } // A API às vezes envia como string "1,749 €"

    // Propriedade auxiliar para cálculos
    //public double Preco => double.TryParse(PrecoString?.Split(' ')[0].Replace(',', '.'), out var res) ? res : 0;
}
