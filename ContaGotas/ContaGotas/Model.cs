namespace ContaGotas;

public class Model
{
    public event Action<List<string>> NotificarTiposDeCombustivel;
    public void ObterTiposDeCombustivel()
    {
        var listaTipos = new List<string> { "Gasolina", "Gasóleo" };
        //Chamada do evento após obter a lista atualizada
        NotificarTiposDeCombustivel?.Invoke(listaTipos);
    }
}