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
    
    //=== CONSTRUTORES ===//
    
    //--- LIMPO ---
    public Model(ICombustivelService service)
    {
        _service = service;
        
    }
    
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

    //=== EVENTOS ===//
    public event Action? OnMediasProntas;
    public event Action<List<TipoCombustivel>, List<Distrito>>? OnTiposDistritos;
    
    public event Action<List<Posto>>? ReadyPostos; 

    
    //=== METODOS ===//
    
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
    
    // --- Pesquisa Distrital ---
    public async Task BuscarTiposDistritos()
    {
        _tiposCombustivel = await _service.ObterTiposAsync();
        _distritos = await _service.ObterDistritosAsync();
        OnTiposDistritos?.Invoke(_tiposCombustivel, _distritos);
    }
    
    
    public async void PesquisarDistritos(int tipo, int distrito)
    {
        List<Posto> postos = await _service.ObterPostosAsync(tipo, distrito);
        ReadyPostos?.Invoke(postos); //previne crash se estiver nulo
        
    }
    
    
    


}