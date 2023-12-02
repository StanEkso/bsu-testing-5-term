namespace DomainLibrary;

public class VariableDef
{
    public VariableDef()
    {
        this.Name = "Undefined";
    }

    public VariableDef(string name)
    {
        this.Name = name;
    }
    
    public VariableDef(string name, double value)
    {
        this.Name = name;
        this.Value = value;
    }

    public string Name
    {
        get;
        set;
    }
    

    public double Value
    {
        get;
        set;
    }

    public VariableDef Clone()
    {
        return new VariableDef(this.Name, this.Value);
    }
}