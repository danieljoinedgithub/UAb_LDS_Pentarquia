using Xunit;

namespace ContaGotas.Tests;

public class PrecoMedioModelTests
{
    [Fact]
    public void IsValido_True_DadosValidos()
    {
        var preco = new PrecoMedioModel("2.10", "Gasolina 95", "Lisboa");

        bool resultado = preco.IsValido();

        Assert.True(resultado);
    }

    [Theory]
    [InlineData("")] //vazio
    [InlineData(" ")]
    [InlineData("0")] //zero
    [InlineData("-1.20")] //negativo
    [InlineData("abc")]
    [InlineData("€")]
    public void IsValido_False_PrecoInvalido(string p)
    {
        var preco = new PrecoMedioModel("", "Gasolina 95", "Lisboa");

        bool resultado = preco.IsValido();

        Assert.False(resultado);
    }

    [Fact]
    public void IsValido_False_CombustivelVazio()
    {
        var preco = new PrecoMedioModel("2.10", "", "Lisboa");

        bool resultado = preco.IsValido();

        Assert.False(resultado);
    }

    [Fact]
    public void IsValido_True_PrecoComEuro()
    {
        var preco = new PrecoMedioModel("2.10 €", "Gasóleo", "Porto");

        bool resultado = preco.IsValido();

        Assert.True(resultado);
    }

    [Fact]
    public void IsValido_True_PrecoComVirgula()
    {
        var preco = new PrecoMedioModel("2,10", "Gasóleo", "Porto");

        bool resultado = preco.IsValido();

        Assert.True(resultado);
    }
}