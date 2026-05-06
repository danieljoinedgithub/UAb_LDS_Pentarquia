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
        model.OnTiposDistritos += ApresentarBoxTipoDistritos;
        model.OnMediasProntas += MostrarMedias;
    }
    
    
    //Eventos
    public event Action<int, int>? Pesquisa;
    
    
    // --- MÉDIAS ---

    private void MostrarMedias()
    {
        var dados = model.ObterMedias();
        
        if (!dados.Any()) {
            Console.WriteLine("Não existem médias disponíveis da DGEG para o período selecionado.");
            return;
        }
        
        Console.WriteLine("Médias obtidas:");
        foreach (var d in dados)
        {
            Console.WriteLine(d.combustivel +" "+ d.valor);
        }
    }

    // Passa a async Task
    // Temos de dizer à View para "esperar" (await) que o Controller acabe 
    // de falar com a API antes de imprimir o menu de novo
    public async Task AtivarInterfaceComOpcoes()
    {
        
        while (true)
        {
            Console.WriteLine("\nMENU:");
            Console.WriteLine("1 - Ver médias");
            Console.WriteLine("2 - Pesquisar distrital (em desenvolvimento)");
            Console.WriteLine("3 - Estatisticas (em desenvolvimento)");
            Console.WriteLine("0 - Sair");

            await SelecionarOpcao();
        }
    }

    // Passa a async Task
    public async Task SelecionarOpcao()
    {
        Console.Write("Escolha opção: ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out int opcao))
        {
            await controller.OpcaoSelecionada(opcao);
        } else {
            Console.WriteLine("Entrada inválida!");
        }
    }

/*+++IMPORTANTE se formos para fazer UI não vamos ter loops o objeto neste caso o dropDownList vai ser configurado par
     buscar a informação diretamente à classe e Onchange(reativo) quando os dois tiveren selecionados pesquisa 
     se for com butao de pesquisa mesma coisa so faz pesquisa quando o utilizador escolhe os dois se por acaso clicar sem
     as escolhas serem completa não faz nada e espera que o utilizador escolha ou volta a traz */
    
    //utilizador clica no butao pesquisa
    public void OnPesquisa(int tipo,int distrito)
    {
        Pesquisa?.Invoke(tipo, distrito);
    }
    
    private void ApresentarBoxTipoDistritos(List<TipoCombustivel> tipos,List<Distrito> distritos)
    {
        /*TODO:condicao para impedir escolha invalida ou um Exception para impedir crash ano sair do loop ate
          opcao valida selecionada*/ 
        
        //Apresentar os tipos de combustível
        Console.WriteLine("\nTIPOS DE COMBUSTÍVEL:");
        foreach (var tipo in tipos)
        {
            Console.WriteLine($"{tipo.Id} - {tipo.Nome}");
        }

        int escolhaTipo = int.Parse(Console.ReadLine());
        

        Console.WriteLine("\nDistritos:");
        foreach (var distrito in distritos)
        {
            Console.WriteLine($"{distrito.Id} - {distrito.Nome}");
        }
        int escolhaDistrito = int.Parse(Console.ReadLine());
        
        //simulacao butao
        OnPesquisa(escolhaTipo, escolhaDistrito);
    }

    public void PresentarResultadoPesquisaDistrital(List<Posto> postos)
    {
        Console.Clear();
        
        foreach (var posto in postos)
        {
            Console.WriteLine(posto.Nome+"   "+posto.Morada+"   "+posto.PrecoString+"€");
        }
        Console.WriteLine("\n prime qualquer tecla para voltar");
        Console.ReadKey(true);
        
        Console.Clear();
    }
}