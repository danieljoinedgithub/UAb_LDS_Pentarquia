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
        model.NotificarDistritos += ApresentarDistritos;
    }
    
    
    //Eventos
    public event Action<int, int>? Pesquisa;
    
    //utilizador clica no butao pesquisa
    public void OnPesquisa()
    {
        //int tipo=campo1.getv
        //int distrito=campo2.getv
        
        int tipo=1;
        int distrito = 1;
        
        Pesquisa?.Invoke(tipo, distrito);
    }
    
    // inicio de esqueleto de processo de obter dados média em view
        _model.OnMudancaEstado += NotificarMudancaEstado;
    

    
    private void NotificarMudancaEstado()
    {
        // A View faz o "Pull" (puxa os dados)
        var dados = _model.ObterDadosMedias();
        MostrarDados(dados);
    }

    private void MostrarDados(List<double> dados)
    {
        Console.WriteLine("Médias obtidas:");
        foreach (var d in dados) Console.WriteLine(d);
    }
    // final de esqueleto de processo de obter dados média em views
    

    public void AtivarInterfaceComOpcoes()
    {
        //Fluxo fechado do menu enquanto não selecionar a opção de sair
        while (true)
        {
            Console.WriteLine("MENU:");
            Console.WriteLine("1 - Ver médias");
            Console.WriteLine("2 - Pesquisar distrital");
            Console.WriteLine("3 - Estatisticas");
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
        if (int.TryParse(input, out int opcao))
        {
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
        Console.WriteLine($"99 - Todos os tipos de combustivel");
        
        //TODO selecionar
    }
    
    private void ApresentarDistritos(List<string> distritos)
    {
        //Apresentar os Distritos
        Console.WriteLine("\nDISTRITOS:");
        for (int i = 0; i < distritos.Count; i++)
        {
            Console.WriteLine($"{i + 1} - {distritos[i]}");
        }
        Console.WriteLine($"99 - Todos os distritos");

        //TODO selecionar
    }
    
}