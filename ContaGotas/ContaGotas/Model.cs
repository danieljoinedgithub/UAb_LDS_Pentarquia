namespace ContaGotas;

public class Model
{
    public event Action<List<string>> NotificarTiposDeCombustivel;
    
    public void ObterTiposDeCombustivel()
    {
        var listaTipos = new List<string> { "Gasolina", "Gasóleo" };
        NotificarTiposDeCombustivel?.Invoke(listaTipos);
    }
}