namespace ContaGotas;

public class View
{
    private Controller controller;
    private Model model;
    public View(Controller c,Model m)
    {
        controller = c;
        model = m;
        
        m.NotificarTiposDeCombustivel += ApresentarTiposCombustivel;
    }

    public void AtivarInterfaceComOpcoes()
    {
        //TODO Ciclo menu?
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
        //TODO Input do utilizador e validações,
        // se valido, chama controller.OpcaoSelecionada(opcao)
        Console.Write("Escolha opção: ");
        string input = Console.ReadLine();
        if (int.TryParse(input, out int opcao))
        {
            if (opcao == 0)
                Environment.Exit(0);
            controller.OpcaoSelecionada(opcao);
        }
        else
        {
            Console.WriteLine("Entrada inválida!");
        }
    }


    private void ApresentarTiposCombustivel(List<string> tipos)
    {
        Console.WriteLine("\nTIPOS DE COMBUSTÍVEL:");
        for (int i = 0; i < tipos.Count; i++)
        {
            Console.WriteLine($"{i + 1} - {tipos[i]}");
        }
    }
}