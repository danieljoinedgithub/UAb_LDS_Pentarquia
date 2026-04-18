namespace ContaGotas;

public class Model
{
    View view;
    Controller controller;
    
    public Model(Controller controller,View view)
    {
        this.view = view;
        this.controller = controller;
    }
    
    public void PesquisarDistritos(int distrito,int id)
    {
        //DGEG.getPostosFilttrados(distrito, id)
        //json
        
    }
    
    public event Action<List<string>> NotificarTiposDeCombustivel;
    public void ObterTiposDeCombustivel()
    {
        var listaTipos = new List<string> { "Gasolina", "Gasóleo" };
        //Chamada do evento após obter a lista atualizada
        NotificarTiposDeCombustivel?.Invoke(listaTipos);
    }
    
    public event Action<List<string>> NotificarDistritos;
    public void ObterDistritos()
    {
        var distritos = new List<string> { "Lisboa", "Porto" };
        //Chamada do evento após obter a lista atualizada
        NotificarDistritos?.Invoke(distritos);
    }
}