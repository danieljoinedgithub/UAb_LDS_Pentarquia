using System.Text.Json;

namespace ContaGotas;

public class PrecoMedioModel
{
    [System.Text.Json.Serialization.JsonPropertyName("PrecoMedio")]
    public string valor { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("TipoCombustivel")]
    public string combustivel { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("Distrito")]
    public string distrito { get; set; }

    public PrecoMedioModel(string valor, string combustivel, string distrito)
    {
        this.valor = valor;
        this.combustivel = combustivel;
        this.distrito = distrito;
    }
}


