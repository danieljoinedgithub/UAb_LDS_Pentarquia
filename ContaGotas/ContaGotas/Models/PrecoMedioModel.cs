namespace ContaGotas;
using System.Text.Json.Serialization;

public class PrecoMedioModel : IValido
{
    [JsonPropertyName("PrecoMedio")]
    public string valor { get; set; }

    [JsonPropertyName("TipoCombustivel")]
    public string combustivel { get; set; }

    [JsonPropertyName("Distrito")]
    public string distrito { get; set; }

    public PrecoMedioModel(string valor, string combustivel, string distrito)
    {
        this.valor = valor;
        this.combustivel = combustivel;
        this.distrito = distrito;
    }


    private decimal getPrecoDecimal()
    {
        string valorDecimal = valor .Replace("€", "").Trim().Replace(".", ",");
        return decimal.Parse(valorDecimal);
    }
    public bool IsValido()
    {
        //Validação do preço preenchido
        if (string.IsNullOrWhiteSpace(valor))
            return false;
        //Validação do valor decimal do preço, maior que zero
        decimal preco = getPrecoDecimal();
        if (preco <= 0)
            return false;

        //Validação do nome/tipo combustivel preenchido
        if (string.IsNullOrWhiteSpace(combustivel))
            return false;

        //Validação do distrito combustivel preenchido
        /*if (string.IsNullOrWhiteSpace(distrito))
            return false;*/

        return true;
    }
}


