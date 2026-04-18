namespace ContaGotas;

public class Controller
{
    Model model;
    View view;

    public Controller()
    {
        model = new Model(this,view); //Model(view)
        view = new View(this, model);
        
        view.Pesquisa += PesquisaDistrital;
    }
    
    public void IniciarAplicacao()
    {
        view.AtivarInterfaceComOpcoes();
    }
    public void TerminarAplicacao()
    {
        Environment.Exit(0);
    }

    public void OpcaoSelecionada(int opcao)
    {
        switch (opcao)
        {
            case 0:
                TerminarAplicacao();
                break;
            
            case 1: // Ver médias
                //model.ObterMedias();
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