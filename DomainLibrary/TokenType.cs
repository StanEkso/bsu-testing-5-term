namespace DomainLibrary;

public enum TokenType
{
    GlobalVariableReference,
    GlobalVariableValue,
    ConditionalGoto,
    Goto,
    Operation,
    ConstantInt,
    Call,
    Return
}