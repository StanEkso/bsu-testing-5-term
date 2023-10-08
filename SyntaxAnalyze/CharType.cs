namespace SyntaxAnalyze;

[Flags]
public enum CharType: byte
{
    Space = 1,
    Operator = 2,
    Digit = 4
}