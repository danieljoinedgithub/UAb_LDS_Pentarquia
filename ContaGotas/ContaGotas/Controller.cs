namespace ContaGotas;

public class Controller
{
    View view;
    Model model;
    public void iniciarAplicacao() {
        model = new Model(this, view);
        view = new View(this, model);

        Console.WriteLine("Bem vindo!");
    }
}