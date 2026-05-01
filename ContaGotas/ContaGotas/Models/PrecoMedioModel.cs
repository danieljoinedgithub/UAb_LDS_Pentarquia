using System.Text.Json.Serialization;
using System.Globalization;

namespace ContaGotas;

public class PrecoMedioModel
{
    [JsonPropertyName("PrecoMedio")]
    public string valorTexto { get; set; } = string.Empty;

    [JsonPropertyName("TipoCombustivel")]
    public string combustivel { get; set; } = string.Empty;

    [JsonPropertyName("Distrito")]
    public string distrito { get; set; } = string.Empty;

    // conversao de string para numero para utilizaçao do view
    public double valor => double.TryParse(valorTexto?.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var res) ? res : 0;

    // Construtor atualizado para usar valorTexto
    public PrecoMedioModel(string valorTexto, string combustivel, string distrito)
    {
        this.valorTexto = valorTexto;
        this.combustivel = combustivel;
        this.distrito = distrito;
    }
}