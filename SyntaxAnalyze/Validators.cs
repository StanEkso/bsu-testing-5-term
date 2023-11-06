namespace SyntaxAnalyze;

public static class Validators
{
    public static bool IsDigit(char @char) => @char is >= '0' and <= '9';

    public static bool IsOperator(char @char)
    {
        char[] operators = { '+', '-', '*', '/' };

        return operators.Contains(@char);
    }
}