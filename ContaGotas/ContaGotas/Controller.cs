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
                    model.ObterTiposDeCombustivel();
                    //model.ObterDistritos();
                    // model.PesquisaDistrital(distrito, id);
                    break;
                
                case 3: //Estatisticas
                    //view.atualizarInterface();
                    model.ObterTiposDeCombustivel();
                    //model.ObterDistritos();
                    break;
                
                default:
                    Console.WriteLine("Opção inválida");
                    break;
            }
        }catch (Exception ex)
        {
            // O tratamento de error
            Console.WriteLine($"\nFalha ao comunicar com o serviço. Detalhes: {ex.Message}");
        }
    }
    
    public void PesquisaDistrital(int distrito,int id)
    {
        model.PesquisarDistritos(distrito, id);
    }
    
}