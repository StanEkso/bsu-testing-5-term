namespace SyntaxAnalyze.Tokens;

public class Compiler
{
    private List<Token> tokens;

    public Compiler()
    {
        tokens = new List<Token>();
    }

    public List<Token> Tokens => tokens;

    public void AddToken(Token token) => tokens.Add(token);

    public void RemoveLastToken() => tokens.RemoveAt(tokens.Count - 1);

    public void RemoveFirstToken() => tokens.RemoveAt(0);

    public void RemoveTokenAt(int index) => tokens.RemoveAt(index - 1);

    public void AddGoto() => tokens.Add(new Token(SyntaxAnalyze.Tokens.Tokens.Goto));

    public void AddGotoIf() => tokens.Add(new Token(SyntaxAnalyze.Tokens.Tokens.GotoIf));

    public void AddCall() => tokens.Add(new Token(SyntaxAnalyze.Tokens.Tokens.Call));

    public void AddReturn() => tokens.Add(new Token(SyntaxAnalyze.Tokens.Tokens.Return));
}