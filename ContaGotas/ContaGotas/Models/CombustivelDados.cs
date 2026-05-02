using System.Text.Json.Serialization;

namespace ContaGotas;


public class TipoCombustivel 
{ 
    [JsonPropertyName("Id")]
    public required int  Id { get; set; } //required para obrigar a preencer com valor
    
    [JsonPropertyName("Descritivo")]
    public string Nome { get; set; } 
}


public class Marca 
{ 
    [JsonPropertyName("ID")]
    public int Id { get; set; } 
    
    [JsonPropertyName("Descricao")]
    public string Nome { get; set; } 
}


public class Distrito 
{ 
    [JsonPropertyName("ID")]
    public int Id { get; set; } 
    
    [JsonPropertyName("Descricao")]
    public string Nome { get; set; } 
}


public class Municipio 
{ 
    [JsonPropertyName("ID")]
    public int Id { get; set; } 
    
    [JsonPropertyName("Descricao")]
    public string Nome { get; set; } 
}


public class Posto 
{ 
    [JsonPropertyName("Nome")]
    public string Nome { get; set; } 
    
    [JsonPropertyName("Morada")]
    public string Morada { get; set; }
    
    [JsonPropertyName("Preco")]
    public required decimal PrecoString { get; set; } // A API às vezes envia como string "1,749 €"

    // Propriedade auxiliar para cálculos
    //public double Preco => double.TryParse(PrecoString?.Split(' ')[0].Replace(',', '.'), out var res) ? res : 0;
}
