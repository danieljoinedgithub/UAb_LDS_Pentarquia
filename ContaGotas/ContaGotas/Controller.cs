using System;
using System.Threading.Tasks;

namespace ContaGotas;
public class Controller
{
    private Model model;

    // O Controller agora só recebe o Model já pronto
    public Controller(Model m)
    {
        model = m;
    }
    
    // inicio de esqueleto de processo de obter dados média em controller
    public void SelecionarOpcao(string opcao)
    {
        if (opcao == "Medias nacionais")
        {
            // Dispara o processo
            model.AtualizarMedias();
        }
    }
    // final de esqueleto de processo de obter dados média em controller
    
    public void TerminarAplicacao()
    {
        Environment.Exit(0);
    }

    // Mudei de "async void" para "async Task" e adicionamos try-catch
    // Quando ligar a API, e a internet for abaixo durante o await model.AtualizarMedias()
    // o programa vai "engolir" o erro e crashar sem aviso, porque o async void não permite que o erro suba para ser tratado.
    
    public async Task OpcaoSelecionada(int opcao)
    {
        try{
            
            
            /*controller não é suposto saber as listas do model caso alguma coisa aconteca o model manda excecao e
              controller apanha o que o model não conseguir gerir ou que não faz parte do papel do model e decide
              o que fazer com a view (destruir, voltar para a view de menu, ou criar uma view com a mensagem de erro) */
            
            switch (opcao)
            {
                case 0:
                    TerminarAplicacao();
                    break;
                
                case 1: // Ver médias
                    
                    await model.AtualizarMedias(); //
                    break;

                case 2: // Pesquisar distrital
                    
                    //view.atualizarInterface();
                    await model.BuscarTiposDistritos();
                    //model.ObterDistritos();
                    
                    //em evento de pesquisa acionada na view//
                    // model.PesquisaDistrital(distrito, id);
                    break;
                
                case 3: //Estatisticas
                    //view.atualizarInterface();
                    //model.ObterTiposDeCombustivel();
                    //model.ObterDistritos();
                    break;
                
                default:
                    //apanha alguma opcao invalida
                    Console.WriteLine($"Opção {opcao} é inválida. Escolha um valor entre 0 e 3.");
                    break;
            }
        }catch (Exception ex)
        {
            /*apanha excecao com mensagem que o service e model não conseguiram ter resultados do
             API impedindo a entrega de uma lista vazia e permitindo ao controler assumir o 
             controlo da excao e controllar o que acontece à view ou ao pedido em si*/
            
            
            if (ex.Message.Contains("Resultados:[]"))
            {
                Console.WriteLine("Dados não carregados Erro: " + ex.Message);
            }
            
            
            // O tratamento de error
            Console.WriteLine($"\nFalha ao comunicar com o serviço. Detalhes: {ex.Message}");
        }
    }
    
    public void PesquisaDistrital(int tipo,int distrito)
    {
        Console.WriteLine("\nid posto:"+tipo+" distrito:"+distrito);
        model.PesquisarDistritos(tipo, distrito);
    }
    
}