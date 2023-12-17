namespace Execution.Compiled;

public abstract class VariableDef
{
     public abstract TypedValue VarValue { get; set; }
}
public class GlobalVariableDef : VariableDef
{
    private TypedValue typedValue;
    public override TypedValue VarValue { get => typedValue; set => typedValue = value; }
}

public class LocalVariableDef : VariableDef
{
    public readonly int index = 0; // left to right in declaration
    public readonly bool isParameter = false;
    public int stackIndex = 0; // StackIndex for caller address to return = 0, for last declared local = 1, etc
    public override TypedValue VarValue { get => GetLocalVarValue(); set => SetLocalVarValue(value); }

    public LocalVariableDef(int Index, bool isParameter)  { this.index = Index; this.isParameter = isParameter; }
        
    private TypedValue GetLocalVarValue() { 
        return new TypedValue(); 
    }
    private void SetLocalVarValue(TypedValue typedValue) { return; }
}


/**
public class VariableDef
{
    public VariableDef(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    public TypedValue typedValue = new TypedValue();
}

public class LocalVariableDef
{
    public VariableDef(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    public TypedValue typedValue = new TypedValue();
}
**/