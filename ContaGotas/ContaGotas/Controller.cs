namespace ContaGotas;

public class Controller
{
    Model model;
    View view;

    public Controller()
    {
        model = new Model(); //Model(view)
        view = new View(this, model);
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
            case 1: // Ver médias
                //model.ObterMedias();
                break;

            case 2: // Pesquisar postos
                model.ObterTiposDeCombustivel();
                break;
            
            case 0:
                TerminarAplicacao();
                break;
            default:
                Console.WriteLine("Opção inválida");
                break;
        }
    }
}