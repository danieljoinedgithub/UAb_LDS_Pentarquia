namespace ContaGotas;

public class View
{
    private Controller controller;
    private Model model;
    public View(Controller c, Model m) {
        controller = c;
        model = m;
    }
}