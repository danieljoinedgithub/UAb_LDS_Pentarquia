namespace ContaGotas;
using Newtonsoft.Json;

public class PrecoMedioModel : IValido
{
    [JsonProperty("PrecoMedio")]
    public string valor { get; set; }

    [JsonProperty("TipoCombustivel")]
    public string combustivel { get; set; }

    [JsonProperty("Distrito")]
    public string distrito { get; set; }

    public decimal? valorAnterior { get; set; }

    public PrecoMedioModel(string valor, string combustivel, string distrito)
    {
        this.valor = valor;
        this.combustivel = combustivel;
        this.distrito = distrito;
    }


    public decimal GetPrecoDecimal()
    {
        string valorDecimal = valor .Replace("€", "").Trim().Replace(".", ",");
        return decimal.Parse(valorDecimal);
    }
    
    
    public decimal GetDiferencaPreco(){
        return valorAnterior.HasValue
            ? GetPrecoDecimal() - valorAnterior.Value
            //? valorAnterior.Value - GetPrecoDecimal()
            : 0;
    }
    
    public bool IsValido()
    {
        //Validação do preço preenchido
        if (string.IsNullOrWhiteSpace(valor))
            return false;
        //Validação do valor decimal do preço, maior que zero
        decimal preco = GetPrecoDecimal();
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


