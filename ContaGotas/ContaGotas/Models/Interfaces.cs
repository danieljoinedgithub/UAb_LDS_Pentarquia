using System;
using System.Threading.Tasks;

namespace ContaGotas;

/*
 * Interface ICombustivelService
 * Permite a abstração se eventualmente trocarmos os dados para persistência ou outra API
 */
public interface ICombustivelService
{
    Task<List<PrecoMedioModel>> ObterMediasAsync(int diasAntes = -7, bool incluirDiferenca = false);
    Task<List<TipoCombustivelModel>> ObterTiposAsync();
    Task<List<DistritoModel>> ObterDistritosAsync();
    Task<List<PostoModel>> ObterPostosAsync(int tipo, int distrito);
}

/*
 * Interface IValido
 * Necessária para garantir a implementação da função de validação dos dados em modelos como dos Preços Médios 
 */
public interface IValido
{
    bool IsValido();
}
