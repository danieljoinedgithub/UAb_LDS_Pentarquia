using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        model.ReadyPostos += ApresentarResultadoPesquisaDistrital;
        
        PesquisaDistrital += controller.PesquisaDistrital;
    }
    
    
    //Eventos
    public event Action<int, int>? PesquisaDistrital;
    

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
            Console.WriteLine("4 - Ver gráfico de médias");
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
            if (opcao == 4)
                MostrarGrafico(); // local, no controller needed
            else
                await controller.OpcaoSelecionada(opcao);
        }
        else
        {
            Console.WriteLine("Entrada inválida!");
        }
    }

    
        
    // --- MÉDIAS ---

    private void MostrarMedias()
    {
        var dados = model.ObterMedias();
        
        if (!dados.Any()) {
            Console.WriteLine("Não existem médias disponíveis da DGEG para o período selecionado.");
            return;
        }
        
        Console.Clear();

        Console.WriteLine("Médias obtidas:");
        foreach (var d in dados)
        {
            decimal diferenca = d.GetDiferencaPreco();
            if (diferenca > 0)
                Console.WriteLine($"{d.combustivel} {d.valor} (+{diferenca:0.000}€ que há 15 dias)");
            else if (diferenca < 0)
                Console.WriteLine($"{d.combustivel} {d.valor} ({diferenca:0.000}€ que há 15 dias)");
            else
                Console.WriteLine($"{d.combustivel} {d.valor}"); // (sem alteração ou sem comparação)
        }

        Console.WriteLine("\n prima qualquer tecla para voltar");
        Console.ReadKey(true);
        
        Console.Clear();
    }
    
/*+++IMPORTANTE se formos para fazer UI não vamos ter loops o objeto neste caso o dropDownList vai ser configurado par
     buscar a informação diretamente à classe e Onchange(reativo) quando os dois tiveren selecionados pesquisa 
     se for com butao de pesquisa mesma coisa so faz pesquisa quando o utilizador escolhe os dois se por acaso clicar sem
     as escolhas serem completa não faz nada e espera que o utilizador escolha ou volta a traz */
    
    //utilizador clica no butao pesquisa
    public void OnPesquisaDistrital(int tipo,int distrito)
    {
        PesquisaDistrital?.Invoke(tipo, distrito);
    }
    
    //Apresenta Lista de distritos do Objeto.Distritos
    private void ApresentarMenuDistritos(List<Distrito> distritos)
    {
        Console.WriteLine("\nDistritos:");
        foreach (var distrito in distritos)
        {
            Console.WriteLine($"{distrito.Id} - {distrito.Nome}");
        }
    }

    /*Apresentar os tipos de combustível
      Sugestoes: Fazer metodos de cada classe para apresentar ou retornar a string para imprimir 
      Exemplo:tipos.toString= return $"{tipo.Id} - {tipo.Nome}");*/
    private void ApresentarMenuTipos(List<TipoCombustivel> tipos)
    {
        
        Console.WriteLine("\nTIPOS DE COMBUSTÍVEL:");
        int i = 1;
        foreach (var tipo in tipos)
        {
            Console.WriteLine($"{i++} - {tipo.Nome}");
        }
    }
    
    private void ApresentarBoxTipoDistritos()
    {
        /*TODO:condicao para impedir escolha invalida ou um Exception para impedir crash áo sair do loop ate
          opcao valida selecionada*/

        while (true)
        {
            try
            {
                List<TipoCombustivel> tipos = model.ObterTipos();
                ApresentarMenuTipos(tipos);
        
                int escolhaTipo = int.Parse(Console.ReadLine());
                int idTipo = tipos[escolhaTipo - 1].Id;
                
                Console.Clear();
                
                List<Distrito> distritos = model.ObterDistritos();
                ApresentarMenuDistritos(distritos); 
                int escolhaDistrito = int.Parse(Console.ReadLine());
                
                Console.Clear();
    
                controller.PesquisaDistrital(idTipo, escolhaDistrito);
                break;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("Escolha um numero valido.\n Qualquer tecla para continuar.");
                Console.ReadKey(true);
            } 
        }
    }

    public void ApresentarResultadoPesquisaDistrital()
    {
        Console.Clear();
        
        List<Posto> postos = model.ObterPostos();
        
        foreach (var posto in postos)
        {
            Console.WriteLine($"Nome:{posto.Nome}\n" +
                              $"Morada:{posto.Morada}\n" +
                              $"Preço:{posto.PrecoString}€");
        }
        Console.WriteLine("\n prime qualquer tecla para voltar");
        /*BUG: a leitura do menu chega primeiro que esse fazendo comportamento imprevisível exemplo:
         
         prime qualquer tecla para voltar
         9                 // tecla escolhida
         Entrada inválida! // saida do menu 
         10                // não aparece no ecran devido ao Console.ReadKey(true); e limpa o ecran
                           // como programado abaixo e espera por input do utilizador que é o menu numa consola limpa
           */
        
        
        Console.ReadLine();
        
        Console.Clear();
    }
    
    public async Task MostrarGrafico()
    {
        //var dados = model.ObterMedias(); para depois a api
        
        var dados = new List<PrecoMedioModel>
        {
            new PrecoMedioModel("1.75", "Gasolina 95", "Lisboa"),
            new PrecoMedioModel("1.62", "Gasoleo", "Porto"),
            new PrecoMedioModel("1.89", "Gasolina 98", "Coimbra"),
            new PrecoMedioModel("1.45", "Gasóleo Colorido", "Faro"),
            new PrecoMedioModel("1.55", "GPL", "Braga")
        };
        //esta lista é usada como valores de teste.

        if (!dados.Any())
        {
            Console.WriteLine("A carregar dados...");
            await controller.OpcaoSelecionada(1);
            dados = model.ObterMedias();
        }

        var plot = new ScottPlot.Plot();


        double[] valores = dados.Select(d => double.Parse(d.valor.ToString(), 
            System.Globalization.CultureInfo.InvariantCulture)).ToArray();
        string[] labels = dados.Select(d => d.combustivel).ToArray();

        // Criar barras
        plot.Add.Bars(valores);

     
        double[] posicoes = Enumerable.Range(0, labels.Length)
            .Select(i => (double)i)
            .ToArray();
        plot.Axes.Bottom.SetTicks(posicoes, labels);
        plot.Axes.Bottom.TickLabelStyle.FontName = "Noto Sans";
        
        plot.Axes.Left.TickLabelStyle.FontName = "Noto Sans";
        plot.Axes.Margins(bottom:0);

        plot.Title("Preços Médios de Combustível (DGEG)");
        plot.YLabel("Preço (€)");

        string caminho = Path.Combine(Path.GetTempPath(), "contagotas_grafico.png");
        plot.SavePng(caminho, 800, 500);

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = caminho,
            UseShellExecute = true
        });

        Console.WriteLine($"Gráfico aberto: {caminho}");
    }
    
}