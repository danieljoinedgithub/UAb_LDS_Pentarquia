namespace ContaGotas;

using System.Threading.Tasks;
using System.Linq;

public class Model
{
    private ICombustivelService _service;

    public event Action? OnMudancaEstado;
    
    // evento quando a pesquisa não encontra nada
    public event Action<string>? OnErroDados;

    private List<PrecoMedioModel> _medias = new();

    // --- CONSTRUTOR LIMPO ---
    public Model(ICombustivelService service)
    {
        _service = service;
    }

    //  MÉDIAS 
    public async Task AtualizarMedias()
    {
        try 
        {
            var dadosApi = await _service.ObterMediasAsync();

            // proteção contra valores null / Lista vazia 
            if (dadosApi == null || !dadosApi.Any())
            {
                _medias = new List<PrecoMedioModel>(); // Garante que não é null
                OnErroDados?.Invoke("Sem dados disponíveis para as médias nacionais.");
                return;
            }

            _medias = dadosApi;
            OnMudancaEstado?.Invoke();
        }
        catch (Exception)
        {
            // Proteção contra falhas de rede
            OnErroDados?.Invoke("Erro de ligação ao servidor da DGEG.");
        }
    }
    

    public List<PrecoMedioModel> ObterMedias()
    {
        // Se for null, devolvemos lista vazia para não crashar a View
        return _medias ?? new List<PrecoMedioModel>();
    }

    // --- OUTRAS FUNCIONALIDADES ---

    public event Action<List<string>>? NotificarTiposDeCombustivel;

    public void ObterTiposDeCombustivel()
    {
        var listaTipos = new List<string> { "Gasolina", "Gasóleo" };
        NotificarTiposDeCombustivel?.Invoke(listaTipos);
    }

    public event Action<List<string>>? NotificarDistritos;

    public void ObterDistritos()
    {
        var distritos = new List<string> { "Lisboa", "Porto" };
        NotificarDistritos?.Invoke(distritos);
    }

    public void PesquisarDistritos(int distrito, int id)
    {
        // Aqui vais depois usar o service para ir à API
        // Exemplo futuro:
        // var dados = await _service.ObterPostosAsync(distrito, id);
    }
    
}