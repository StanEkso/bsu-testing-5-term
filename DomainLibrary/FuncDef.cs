namespace DomainLibrary;

public class FuncDef
{
    public Dictionary<string, VariableDef> localVariables = new ();

    public FuncDef()
    {
        this.Name = "Undefined";
        this.ParamCount = 0;
    }

    public FuncDef(string name)
    {
        this.Name = name;
    }
    
    public string Name
    {
        set;
        get;
    }

    public int ParamCount
    {
        set;
        get;
    }
}