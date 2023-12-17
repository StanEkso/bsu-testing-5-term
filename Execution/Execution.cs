using static System.Runtime.InteropServices.JavaScript.JSType;
using Execution.Compiled;
using Execution.SyntaxAnalyze;
//using Execution.;

namespace Execution;
public static class Execution
{
    public static TypedValue? Exec(string source)
    {
        var parser = new Analyzer(source);

        if (!parser.Parse())
            return null;
        var calculator = new Calculator(parser.CompiledCode);
        return calculator.Compute();
    }

    public static TypedValue? CalcExpression(string source)
    {
        var parser = new Analyzer(source);

        if (!parser.IsValidExpression())
            return null;
        var calculator = new Calculator(parser.CompiledCode);
        return calculator.Compute();
    }

}