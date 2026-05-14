using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContaGotas;

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
        
        _medias = await _service.ObterMediasAsync(-7, true);
        OnMediasProntas?.Invoke();
    }

    public List<PrecoMedioModel> ObterMedias()
    {
        return _medias;
    }
    
    // --- Pesquisa Distrital ---
    public async Task BuscarTiposDistritos()
    {
        if (_tiposCombustivel.Count == 0 && _distritos.Count == 0)
        {
            Task<List<TipoCombustivel>> tiposGet = _service.ObterTiposAsync();
            Task<List<Distrito>> distritosGet = _service.ObterDistritosAsync();
            await Task.WhenAll(tiposGet, distritosGet);
        
        _tiposCombustivel = await tiposGet;
        _distritos = await distritosGet;
        }
        
        
        
        
        OnTiposDistritos?.Invoke(_tiposCombustivel, _distritos);
    }
    
    
    public async Task PesquisarDistritos(int tipo, int distrito)
    {
        List<Posto> postos = await _service.ObterPostosAsync(tipo, distrito);
        ReadyPostos?.Invoke(postos); //previne crash se estiver nulo
        
    }
    
    
    


}