namespace ContaGotas;

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class Model
{
    View view;
    Controller controller;
    
    public Model(Controller controller,View view)
    {
        this.view = view;
        this.controller = controller;
    }
    
    // inicio de esqueleto de processo de obter dados média em model
    public event Action OnMudancaEstado;
    
    private List<double> _listaMedias = new List<double>();

    public void ActualizarMediasNacionais()
    {
        // Corresponde ao "CarregarDados()" no UML
        // Simulação de carregamento de dados da API
        _listaMedias = new List<double> { 1.55, 1.62, 1.48 }; 
        
        // Notifica a View que algo mudou
        OnMudancaEstado?.Invoke(); 
    }

    public List<double> ObterDadosMedias()
    {
        // Corresponde ao retorno "listaMedias" no UML
        return _listaMedias;
    }
    }
    // final de esqueleto de processo de obter dados média em model
    
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