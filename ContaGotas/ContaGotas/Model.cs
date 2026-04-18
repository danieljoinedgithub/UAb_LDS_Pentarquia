namespace ContaGotas;

public class Model
{
    private Controller controller;
    private View view;
    public Model(Controller c, View v) {
        controller = c;
        view = v;
    }
}