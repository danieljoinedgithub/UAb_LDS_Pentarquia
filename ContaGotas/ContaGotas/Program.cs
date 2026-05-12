using System.Threading.Tasks;

namespace ContaGotas;

public class Program
{
    static async Task Main(string[] args) {
        // Criamos o serviço real
        var service = new CombustivelApiService();
        
        // Criamos o Model com esse serviço
        var model = new Model(service);
        
        // Criamos o Controller e damos-lhe o Model
        var controller = new Controller(model);
        
        // Criamos a View e damos-lhe o Controller e o Model
        var view = new View(controller, model);
        
        
        //Como está estruturado obriga-me a subscrever o metodo do model ao evento da view aqui vamos ter de mudar isto

 
        
        // A View inicia o ciclo de vida (C&G)
        await view.AtivarInterfaceComOpcoes();
    }
}