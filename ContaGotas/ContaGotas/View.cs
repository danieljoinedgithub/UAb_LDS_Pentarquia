namespace ContaGotas;

using ZedGraph;
using System.Drawing;
using System;
using System.Threading;          
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Runtime.InteropServices;



public class View : Form
{
    
    bool graficoAberto = false;
    private Controller controller;
    private Model model;
    private string opcao = "";
    
    private ZedGraphControl zedGraphControl;
    private bool _isUpdating = false; //para prevenir crash de clicar muitas vezes
    
    public View(Controller c,Model m)
    {
        controller = c;
        model = m;
        
        // Inicialização do componente gráfico
        zedGraphControl = new ZedGraphControl();
        zedGraphControl.Dock = DockStyle.Fill; 
        this.Controls.Add(zedGraphControl);
        ConfigurarEstiloGrafico();
        
        //Subscrição dos eventos necessarios entre View-Model
        model.NotificarTiposDeCombustivel += ApresentarTiposCombustivel;
        model.NotificarDistritos += ApresentarDistritos;
        model.OnMudancaEstado += NotificarMudancaEstado;
        
        // em caso de erro de dados
        model.OnErroDados += MostrarMensagemErro;
        
        
        
        
        
        // --- TESTE DO ZEDGRAPH ---
        var dadosTeste = new List<PrecoMedioModel> {
            new PrecoMedioModel("1.75", "Gasolina 95", "Lisboa"),
            new PrecoMedioModel("1.62", "Gasóleo", "Porto"),
            new PrecoMedioModel("1.89", "Gasolina 98", "Coimbra")
        };

        AtualizarGrafico(dadosTeste);
        // -------------------------
        
    }
    
    private void ConfigurarEstiloGrafico()
    {
        GraphPane myPane = zedGraphControl.GraphPane;
        myPane.Title.Text = "Evolução de Preços Médios";
        myPane.XAxis.Title.Text = "Combustível";
        myPane.YAxis.Title.Text = "Preço (€)";
        
        // Risco: Eixo mal configurado -> Forçar escala a começar no zero
        myPane.YAxis.Scale.Min = 0;
    }
    
    private void AtualizarGrafico(List<PrecoMedioModel> dados)
    {
        GraphPane myPane = zedGraphControl.GraphPane;
        
        // Risco: Crash ao redesenhar -> Limpar sempre antes de adicionar
        myPane.CurveList.Clear();

        // Recomendação: Validar se há dados (Tarefa 2)
        if (dados.Count == 0)
        {
            Console.WriteLine("[Aviso Gráfico] Sem pontos para desenhar.");
            return;
        }

        PointPairList listaPontos = new PointPairList();
        double x = 1;

        foreach (var d in dados)
        {
            // Forçamos a conversão para double aqui
            listaPontos.Add(x, (double)d.valor); 
            x++;
        }

        // Criar a curva (Barras ou Linhas)
        LineItem minhaCurva = myPane.AddCurve("Preço", listaPontos, Color.Blue, SymbolType.Circle);
        
        // Risco: Gráfico sem pontos -> Avisar o componente para recalcular escalas
        zedGraphControl.AxisChange();
        Console.WriteLine("[ZedGraph] Gráfico atualizado com sucesso.");
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
    
    // --- MÉDIAS ---

    private void NotificarMudancaEstado()
    {
        var dados = model.ObterMedias();
        
        // Mostrar na consola
        MostrarDados(dados);
        
        // Mostrar no Gráfico
        AtualizarGrafico(dados);
        
        _isUpdating = false; // Libertar bloqueio (Tarefa 1)
    }
    
    private void MostrarMensagemErro(string mensagem)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[ERRO]: {mensagem}");
        Console.ResetColor();
        _isUpdating = false; // Libertar bloqueio em caso de falha
    }

    private void MostrarDados(List<PrecoMedioModel> dados)
    {
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
            Console.WriteLine("2 - Pesquisar distrital");
            Console.WriteLine("3 - Estatisticas");
            Console.WriteLine("4 - grafico"); //iremos alterar de lugar depois, só para testar
            Console.WriteLine("0 - Sair");

            string? leitura = Console.ReadLine();
            this.opcao = leitura ?? "";
            

            if (this.opcao == "4") 
            {
                
                // VERIFICAÇÃO DE SEGURANÇA PARA MACOS
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n[AVISO]: O gráfico (ZedGraph) apenas é compatível com Windows.");
                    Console.WriteLine("No MacOS, por favor consulte os dados através das opções de texto.");
                    Console.ResetColor();
                    continue; // Volta ao menu 
                }
                if (graficoAberto) { Console.WriteLine("\nAviso: O gráfico já está aberto."); continue; }

                // 1. Obtemos os dados antes de abrir a thread
                var dadosParaExibir = controller.ObterDadosParaGrafico("sucesso"); 

                Thread t = new Thread(() => {
                    try 
                    {
                        graficoAberto = true;
                        Application.EnableVisualStyles();
            
                        // 2. Usamos o construtor de 2 argumentos que JÁ EXISTE (resolve o erro :193)
                        View formGrafico = new View(this.controller, this.model);
            
                        // 3. Injetamos os dados diretamente no método de desenho
                        formGrafico.AtualizarGrafico(dadosParaExibir);
            
                        Application.Run(formGrafico); 
                    }
                    catch (Exception ex) { Console.WriteLine("\n[Erro]: " + ex.Message); }
                    finally { graficoAberto = false; }
                });

                t.SetApartmentState(ApartmentState.STA);
                t.IsBackground = true; 
                t.Start();
            }

            if (this.opcao == "0") break;
        }
    }

    // Passa a async Task
    public async Task SelecionarOpcao()
    {
        if (_isUpdating) 
        {
            Console.WriteLine("\nAguarde, o pedido anterior ainda está a ser processado...");
            return;
        }
        
        Console.Write("Escolha opção: ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out int opcao))
        {
            // Ativamos o bloqueio antes de chamar o controller
            _isUpdating = true; 
        
            try 
            {
                await controller.OpcaoSelecionada(opcao);
            }
            catch (Exception ex)
            {
                // Se algo falhar no controller/API, garantimos que o bloqueio é libertado
                MostrarMensagemErro("Erro inesperado no processamento.");
                _isUpdating = false;
            }
        } 
        else 
        {
            Console.WriteLine("Entrada inválida! Por favor, insira um número.");
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