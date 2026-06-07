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
    private bool _consoleLock = false; //condição de corrido com leitura de consola
    
    public View(Controller c, Model m)
    {
        controller = c;
        model = m;
        
        //Subscrição dos eventos necessarios entre View-Model
        model.OnTiposDistritos += ApresentarBoxTipoDistritos;
        model.OnMediasProntas += MostrarMedias;
        model.ReadyPostos += ApresentarResultadoPesquisaDistrital;
        
        //Subscrição dos eventos necessarios entre View-Controller
        controller.OnErroOcorrido += MostrarErro;
        
        PesquisaDistrital += controller.PesquisaDistrital;
    }
    
    
    private void MostrarErro(string mensagemErro)
    {
        Console.WriteLine($"\n{mensagemErro}");
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
            if (!_consoleLock)
            {
                Console.WriteLine("\nMENU:");
                Console.WriteLine("1 - Ver médias");
                Console.WriteLine("2 - Pesquisar distrital");
                Console.WriteLine("3 - Ver gráfico de médias (barras)");
                Console.WriteLine("4 - Ver gráfico de evolução de preços (linha)");
                Console.WriteLine("5 - Ver gráfico distrital - top 4 mais baratos");
                Console.WriteLine("0 - Sair");

                await SelecionarOpcao();
            }
            Task.Delay(500).Wait();
        }
    }
    
    // Passa a async Task
    public async Task SelecionarOpcao()
    {
        Console.Write("Escolha opção: ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out int opcao))
        {
            if (opcao == 3)
                await MostrarGrafico();
            else if (opcao == 4)
                await MostrarGraficoEvolucao();
            else if (opcao == 5)
                await MostrarGraficoDistrital();
            else
            {
                _consoleLock = true;
                await controller.OpcaoSelecionada(opcao);
            }
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

        Console.WriteLine("MÉDIAS OBTIDAS:");
        foreach (var d in dados)
        {
            decimal diferenca = d.GetDiferencaPreco();
            if (diferenca > 0)
                Console.WriteLine($"{d.combustivel} {d.valor} (+{diferenca:0.000}€ que há 15 dias)");
            else if (diferenca < 0)
                Console.WriteLine($"{d.combustivel} {d.valor} ({diferenca:0.000}€ que há 15 dias)");
            else
                Console.WriteLine($"{d.combustivel} {d.valor}");
        }

        Console.WriteLine("\n prima qualquer tecla para voltar");
        Console.ReadKey(true);
        Console.Clear();
        _consoleLock = false;
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
    private void ApresentarMenuDistritos(List<DistritoModel> distritos)
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
    private void ApresentarMenuTipos(List<TipoCombustivelModel> tipos)
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
        while (true)
        {
            try
            {
                List<TipoCombustivelModel> tipos = model.ObterTipos();
                ApresentarMenuTipos(tipos);
        
                int escolhaTipo = int.Parse(Console.ReadLine());
                int idTipo = tipos[escolhaTipo - 1].Id;
                
                Console.Clear();
                
                List<DistritoModel> distritos = model.ObterDistritos();
                ApresentarMenuDistritos(distritos); 
                int escolhaDistrito = int.Parse(Console.ReadLine());
                
                Console.Clear();
    
                controller.PesquisaDistrital(idTipo, escolhaDistrito);
                break;
            }
            catch (Exception ex) when(ex is FormatException || ex is ArgumentOutOfRangeException)
            {
                Console.WriteLine("Escolha um numero valido.\n Qualquer tecla para continuar.");
                Console.ReadKey(true);
            } 
        }
    }

    public void ApresentarResultadoPesquisaDistrital()
    {
        Console.Clear();
        
        List<PostoModel> postos = model.ObterPostos();
        
        foreach (var posto in postos)
        {
            Console.WriteLine($"Nome:{posto.nome}\n" +
                              $"Morada:{posto.morada}\n" +
                              $"Preço:{posto.preco}€");
        }
        Console.WriteLine("\n prime qualquer tecla para voltar");
        Console.ReadKey(true);
        Console.Clear();
        _consoleLock = false;
    }

    // --- GRÁFICO BARRAS (opção 4) ---
    public async Task MostrarGrafico()
    {
        var dados = model.ObterMedias();

        if (!dados.Any())
        {
            Console.WriteLine("A carregar dados...");
            await controller.OpcaoSelecionada(1);
            dados = model.ObterMedias();
        }

        if (!dados.Any())
        {
            Console.WriteLine("[Erro] Não foi possível obter dados da DGEG.");
            return;
        }

        try
        {
            var dadosValidos = dados.Where(d =>
                !string.IsNullOrWhiteSpace(d.valor) &&
                !string.IsNullOrWhiteSpace(d.combustivel) &&
                double.TryParse(
                    d.valor.Replace("€", "").Replace(",", ".").Trim(),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out _)
            ).ToList();

            double[] valores = dadosValidos.Select(d =>
                double.Parse(d.valor.Replace("€", "").Replace(",", ".").Trim(),
                    CultureInfo.InvariantCulture)).ToArray();
            string[] labels = dadosValidos.Select(d => d.combustivel).ToArray();

            var plot = new ScottPlot.Plot();
            plot.Add.Bars(valores);

            double[] posicoes = Enumerable.Range(0, labels.Length).Select(i => (double)i).ToArray();
            plot.Axes.Bottom.SetTicks(posicoes, labels);
            plot.Axes.Bottom.TickLabelStyle.FontName = "Noto Sans";
            plot.Axes.Bottom.TickLabelStyle.FontSize = 9;
            plot.Axes.Left.TickLabelStyle.FontName = "Noto Sans";
            plot.Axes.Left.TickLabelStyle.FontSize = 9;
            plot.Axes.Margins(bottom: 0);
            plot.Title("Preços Médios de Combustível (DGEG)");
            plot.YLabel("Preço (€)");

            AbrirGrafico(plot, "contagotas_medias.png");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Erro] Não foi possível gerar o gráfico: {ex.Message}");
            Console.ResetColor();
        }
    }

    // --- GRÁFICO EVOLUÇÃO (opção 5) ---
    public async Task MostrarGraficoEvolucao()
    {
        // Carregar tipos se ainda não estiverem disponíveis
        var tipos = model.ObterTipos();
        if (!tipos.Any())
        {
            Console.WriteLine("A carregar tipos de combustível...");
            await controller.OpcaoSelecionada(2);
            tipos = model.ObterTipos();
        }

        // Mostrar tipos disponíveis e pedir seleção
        Console.WriteLine("\nSelecione os combustíveis a comparar (separados por vírgula, ex: 1,3):");
        ApresentarMenuTipos(tipos);
        Console.Write("Escolha: ");
        string input = Console.ReadLine() ?? "";

        var indices = input.Split(',')
            .Select(s => int.TryParse(s.Trim(), out int n) ? n - 1 : -1)
            .Where(i => i >= 0 && i < tipos.Count)
            .ToList();

        if (!indices.Any())
        {
            Console.WriteLine("Nenhum combustível válido selecionado.");
            return;
        }

        var nomesAlvo = indices.Select(i => tipos[i].Nome).ToList();

        Console.WriteLine("A carregar dados históricos...");

        List<PrecoMedioModel> hoje = new();
        List<PrecoMedioModel> semana = new();
        List<PrecoMedioModel> duasSem = new();

        try { hoje = await model.ObterMediasDataAsync(-1); } catch { }
        try { semana = await model.ObterMediasDataAsync(-8); } catch { }
        try { duasSem = await model.ObterMediasDataAsync(-15); } catch { }

        if (!hoje.Any() && !semana.Any() && !duasSem.Any())
        {
            Console.WriteLine("[Erro] Não foi possível obter dados históricos da DGEG.");
            return;
        }

        try
        {
            var plot = new ScottPlot.Plot();
            double[] xs = { 0, 1, 2 };
            string[] eixoX = { "Há 2 semanas", "Há 1 semana", "Ontem" };

            foreach (var nome in nomesAlvo)
            {
                double[] ys = new double[3];
                ys[0] = ExtrairPreco(duasSem, nome);
                ys[1] = ExtrairPreco(semana, nome);
                ys[2] = ExtrairPreco(hoje, nome);

                if (ys.Any(v => v > 0))
                {
                    var linha = plot.Add.Scatter(xs, ys);
                    linha.LegendText = nome;
                    linha.MarkerSize = 8;
                }
            }

            plot.Axes.Bottom.SetTicks(xs, eixoX);
            plot.Axes.Bottom.TickLabelStyle.FontName = "Noto Sans";
            plot.Axes.Left.TickLabelStyle.FontName = "Noto Sans";
            plot.Title("Evolução de Preços — Últimas 2 Semanas (DGEG)");
            plot.YLabel("Preço (€)");
            plot.ShowLegend();

            AbrirGrafico(plot, "contagotas_evolucao.png");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Erro] Não foi possível gerar o gráfico: {ex.Message}");
            Console.ResetColor();
        }
    }

    // --- GRÁFICO DISTRITAL (opção 6) ---
    public async Task MostrarGraficoDistrital()
    {
        var tipos = model.ObterTipos();
        var distritos = model.ObterDistritos();

        if (!tipos.Any() || !distritos.Any())
        {
            Console.WriteLine("A carregar tipos e distritos...");
            await controller.OpcaoSelecionada(2);
            tipos = model.ObterTipos();
            distritos = model.ObterDistritos();
        }

        // Selecionar tipo
        ApresentarMenuTipos(tipos);
        Console.Write("Escolha o tipo de combustível: ");
        if (!int.TryParse(Console.ReadLine(), out int idxTipo) || idxTipo < 1 || idxTipo > tipos.Count)
        {
            Console.WriteLine("Escolha inválida.");
            return;
        }
        int idTipo = tipos[idxTipo - 1].Id;

        // Selecionar distrito
        ApresentarMenuDistritos(distritos);
        Console.Write("Escolha o distrito: ");
        if (!int.TryParse(Console.ReadLine(), out int idDistrito))
        {
            Console.WriteLine("Escolha inválida.");
            return;
        }

        Console.WriteLine("A carregar postos...");
        await model.PesquisarDistritos(idTipo, idDistrito);
        var postos = model.ObterPostos();

        if (postos == null || !postos.Any())
        {
            Console.WriteLine("Nenhum posto encontrado.");
            return;
        }

        try
        {
            var postosValidos = postos
                .Where(p => !string.IsNullOrWhiteSpace(p.nome) && p.preco > 0)
                .OrderBy(p => p.preco)
                .Take(4)
                .ToList();

            if (!postosValidos.Any())
            {
                Console.WriteLine("Sem dados válidos para gerar o gráfico.");
                return;
            }

            double[] valores = postosValidos.Select(p => (double)p.preco).ToArray();
            string[] labels = postosValidos.Select(p => p.nome).ToArray();

            var plot = new ScottPlot.Plot();
            plot.Add.Bars(valores);

            double[] posicoes = Enumerable.Range(0, labels.Length).Select(i => (double)i).ToArray();
            plot.Axes.Bottom.SetTicks(posicoes, labels);
            plot.Axes.Bottom.TickLabelStyle.FontName = "Noto Sans";
            plot.Axes.Bottom.TickLabelStyle.FontSize = 9;
            plot.Axes.Bottom.TickLabelStyle.Rotation = 0;
            plot.Axes.Left.TickLabelStyle.FontName = "Noto Sans";
            plot.Axes.Left.TickLabelStyle.FontSize = 9;
            plot.Axes.Margins(bottom: 0);

            double min = valores.Min();
            double max = valores.Max();
            double margem = (max - min) * 0.5;
            if (margem < 0.05) margem = 0.05;
            plot.Axes.SetLimitsY(min - margem, max + margem);

            plot.Title("Top 4 Postos mais Baratos (DGEG)");
            plot.YLabel("Preço (€)");

            AbrirGrafico(plot, "contagotas_distrital.png");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Erro] Não foi possível gerar o gráfico: {ex.Message}");
            Console.ResetColor();
        }
    }

    // --- HELPER: renderizar e abrir PNG ---
    private void AbrirGrafico(ScottPlot.Plot plot, string nomeFile)
    {
        string caminho = Path.Combine(Path.GetTempPath(), nomeFile);
        plot.SavePng(caminho, 800, 500);

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = caminho,
            UseShellExecute = true
        });

        Console.WriteLine($"Gráfico aberto: {caminho}");
    }

    // --- HELPER: extrair preço de uma lista por nome ---
    private double ExtrairPreco(List<PrecoMedioModel> lista, string nomeCombustivel)
    {
        var item = lista.FirstOrDefault(d =>
            d.combustivel.Equals(nomeCombustivel, StringComparison.OrdinalIgnoreCase));

        if (item == null) return 0;

        string limpo = item.valor.ToString().Replace("€", "").Replace(",", ".").Trim();
        return double.TryParse(limpo, NumberStyles.Any,
            CultureInfo.InvariantCulture, out double result) ? result : 0;
    }
}