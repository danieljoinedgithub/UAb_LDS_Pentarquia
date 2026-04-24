namespace ContaGotas;

public class Controller
{
    Model model;
    View view;

    public Controller()
    {
        var service = new CombustivelApiService();
        model = new Model(service);
        view = new View(this, model);
        
        view.Pesquisa += PesquisaDistrital;
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
    
    public void IniciarAplicacao()
    {
        view.AtivarInterfaceComOpcoes();
    }
    public void TerminarAplicacao()
    {
        Environment.Exit(0);
    }

    public async void OpcaoSelecionada(int opcao)
    {
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
    }
    
    public void PesquisaDistrital(int distrito,int id)
    {
        model.PesquisarDistritos(distrito, id);
    }
    
}