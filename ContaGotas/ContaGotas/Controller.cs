namespace ContaGotas;

public class Controller
{
    Model model;
    View view;

    public Controller()
    {
        view = new View(model);
        model = new Model(view);
    }
    
    public void iniciarAplicacao() {
        Console.WriteLine("Bem vindo!");
    }
}