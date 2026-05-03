namespace ContaGotas;

using System.Threading.Tasks;

public class Model
{
    private ICombustivelService _service;
    
    //=== Listas para dados ===//
    private List<PrecoMedioModel> _medias = new();
    private List<TipoCombustivel> _tiposCombustivel = new();
    private List<Distrito> _distritos = new();
    private List<Posto> _postosPesquisados = new();
    
    
    //=== GETS ===//
    public List<PrecoMedioModel> GetMedias()
    {
        return _medias;
    }

    public List<TipoCombustivel> GetTipos()
    {
        return _tiposCombustivel;
    }

    public List<Distrito> GetDistritos()
    {
        return _distritos;
    }

    public List<Posto> GetPostos()
    {
        return _postosPesquisados;
    }

    public event Action? OnMediasProntas;

    

    // --- CONSTRUTOR LIMPO ---
    public Model(ICombustivelService service)
    {
        _service = service;
    }

    // --- MÉDIAS (ligado ao Service) ---
    public async Task AtualizarMedias()
    {
        
        _medias = await _service.ObterMediasAsync();
        OnMediasProntas?.Invoke();
    }

    public List<PrecoMedioModel> ObterMedias()
    {
        return _medias;
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