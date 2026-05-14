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
    
    public List<PrecoMedioModel> ObterMedias()
    {
        return _medias;
    }

    public List<TipoCombustivel> ObterTipos()
    {
        return _tiposCombustivel;
    }

    public List<Distrito> ObterDistritos()
    {
        return _distritos;
    }

    public List<Posto> ObterPostos()
    {
        return _postosPesquisados;
    }

    //=== EVENTOS ===//
    public event Action? OnMediasProntas;
    public event Action? OnTiposDistritos;
    
    public event Action? ReadyPostos; 

    
    //=== METODOS ===//
    
    // --- MÉDIAS (ligado ao Service) ---
    public async Task AtualizarMedias()
    {
        
        _medias = await _service.ObterMediasAsync();
        OnMediasProntas?.Invoke();
    }
    
    // --- Pesquisa Distrital ---
    public async Task BuscarTiposDistritos(bool force = false)
    {
        if (_tiposCombustivel.Count == 0 && _distritos.Count == 0 || force)
        {
            Task<List<TipoCombustivel>> tiposGet = _service.ObterTiposAsync();
            Task<List<Distrito>> distritosGet = _service.ObterDistritosAsync();
            await Task.WhenAll(tiposGet, distritosGet);
        
            _tiposCombustivel = await tiposGet;
            _distritos = await distritosGet;
        }
        OnTiposDistritos?.Invoke();
    }
    
    
    public async Task PesquisarDistritos(int tipo, int distrito)
    {
        _postosPesquisados = await _service.ObterPostosAsync(tipo, distrito);
        ReadyPostos?.Invoke(); //previne crash se estiver nulo
        
    }
    
    
    


}