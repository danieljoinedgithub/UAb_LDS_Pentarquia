namespace ContaGotas;
public class Program
{
    Controller controller = new Controller();
    static void Main(string[] args) {
        Program app = new Program();
        // Iniciar o programa
        app.controller.iniciarAplicacao();
    }
}