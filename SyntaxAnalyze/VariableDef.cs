namespace SyntaxAnalyze;

public class VariableDef
{
    public string Name { get; init; }

    public Variable Variable { get; set; }

    public VariableDef(string name)
    {
        Name = name;
    }
}