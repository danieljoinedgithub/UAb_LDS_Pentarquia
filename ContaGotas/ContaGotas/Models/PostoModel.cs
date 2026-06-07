using System.Globalization;
using Newtonsoft.Json;

namespace ContaGotas;

public class PostoModel : IValido
{ 
    [JsonProperty("Id", Required = Required.Always)]
    public int  id { get; set; } //required para obrigar a preencer com valor
    
    [JsonProperty("Nome")]
    public string nome { get; set; } 
    
    [JsonProperty("Morada")]
    public string morada { get; set; }
    
    [JsonProperty("Preco", Required = Required.Always)]
    public decimal preco { get; set; } // A API às vezes envia como string "1,749 €"
    
    [JsonProperty("Latitude")]
    public double latitude { get; set;}
   
    [JsonProperty("Longitude")]
    public double longitude { get; set; }
    
    public bool IsValido()
    {
        //Validação ID
        if (id < 0 )
            return false;
        //Validação do Nome
        if (string.IsNullOrWhiteSpace(nome))
            return false;
        if (string.IsNullOrWhiteSpace(morada))
            return false;
        if (preco <= 0)
            return false;

        return true;
    }
    //para o float não vir com ',' para abrir o mapa 
    [JsonIgnore] 
    public string LatitudeToString => latitude.ToString(CultureInfo.InvariantCulture);
    [JsonIgnore] 
    public string LongitudeToString => longitude.ToString(CultureInfo.InvariantCulture);

    // Propriedade auxiliar para cálculos
    //public double Preco => double.TryParse(PrecoString?.Split(' ')[0].Replace(',', '.'), out var res) ? res : 0;
}