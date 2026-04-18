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
    
    public void PesquisaDgeg(int distrito,int id)
    {
        //DGEG
        //json
        
    }
    
    
    
    public event Action<List<string>> NotificarTiposDeCombustivel;
    public void ObterTiposDeCombustivel()
    {
        var listaTipos = new List<string> { "Gasolina", "Gasóleo" };
        //Chamada do evento após obter a lista atualizada
        NotificarTiposDeCombustivel?.Invoke(listaTipos);
    }
}