namespace ContaGotas;

public class View
{
    private Controller controller;
    private Model model;
    
    public View(Controller c,Model m)
    {
        controller = c;
        model = m;
        
        //Subscrição dos eventos necessarios entre View-Model
        model.NotificarTiposDeCombustivel += ApresentarTiposCombustivel;
    }

    public void AtivarInterfaceComOpcoes()
    {
        //Fluxo fechado do menu enquanto não selecionar a opção de sair
        while (true)
        {
            Console.WriteLine("MENU:");
            Console.WriteLine("1 - Ver médias");
            Console.WriteLine("2 - Pesquisar postos");
            Console.WriteLine("0 - Sair");

            SelecionarOpcao();
        }
    }
    public void SelecionarOpcao()
    {
        //Obter input do utilizador
        Console.Write("Escolha opção: ");
        string input = Console.ReadLine();

        //Validações da opção selecionada (se 0 ou inválida)
        if (int.TryParse(input, out int opcao)) {
            if (opcao == 0)
                Environment.Exit(0);
            //Notificar o controller da opção selecionada
            controller.OpcaoSelecionada(opcao);
        } else {
            Console.WriteLine("Entrada inválida!");
        }
    }


    private void ApresentarTiposCombustivel(List<string> tipos)
    {
        //Apresentar os tipos de combustível
        Console.WriteLine("\nTIPOS DE COMBUSTÍVEL:");
        for (int i = 0; i < tipos.Count; i++)
        {
            Console.WriteLine($"{i + 1} - {tipos[i]}");
        }
    }
}