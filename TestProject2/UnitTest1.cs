namespace TestProject2;

public class Tests
{
    [TestCase("x=14+4;", true)]
    [TestCase(
        """
        x=14+4;
        x=x;
        """
      , true)]
    [TestCase(
        """
        x= 14 + 4;
        y= x + 1;
        z = 0;
        z = 10;
        """
      , true)]
    public void ValidatesParse(string expression, bool expected)

    {
        var parser = new SyntaxAnalyze.Analyzer(expression);
        bool actual = parser.Parse();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase("x=y4;")]
    [TestCase("x=x;")]
    public void ValidatesParseThrowException(string expression)
    {
        var parser = new SyntaxAnalyze.Analyzer(expression);
        Assert.Throws(typeof(KeyNotFoundException), () => parser.Parse());
    }


    [TestCase("2 + 3", true)]
    [TestCase("2 +\n 3", true)]
    [TestCase("2 + x3", true)]
    [TestCase("2 +\n x3", true)]
    [TestCase("24 + _x3", true)]
    [TestCase("24 + x_123", true)]
    [TestCase("24 + x_123\n", true)]
    [TestCase("24 + y12", true)]
    [TestCase("24 + 3 + 9", true)]
    [TestCase("24 + 3\n + 9", true)]
    [TestCase("24 + 3 + 9\t", true)]
    [TestCase("24 + (3 + 5)", true)]
    [TestCase("24 + (3 * 6)", true)]
    [TestCase("24 + (3\n\r * 6)", true)]
    [TestCase("24 + (x * y)", true)]
    [TestCase("24 + (x\r\n * y)", true)]
    [TestCase("24 + (x * 12)", true)]
    [TestCase("24 + (x* y)", true)]
    [TestCase("24 + (x*y)", true)]
    [TestCase("24 + (x\r*y)", true)]
    [TestCase("24 + (x_*_y)", true)]
    [TestCase("24 \n + \n (x_*_y)", true)]
    [TestCase("24 \t + \r\n (3 + (5 * 4) * 4 - 4 * (2 + 3))", true)]
    [TestCase("24 + (3 + (5 * 4) * 4 - 4 * (2 + 3))", true)]
    [TestCase("24 + (3 + (5 * x) * 4 - 4 * (y + 3))", true)]
    public void ValidatesExpression(string expression, bool expected)

    {
        var parser = new SyntaxAnalyze.Analyzer(expression);
        bool actual = parser.IsValidExpression();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase(
        """
        2 + 3 // Hello world
        // It's test
        + 4
        """)]
    [TestCase(
        """
        2 + 3 // Hello world
        + 4
        """)]
    [TestCase(
        """
        2 + 3 // Hello world 
                     // It's test
        + 4
        """)]
    [TestCase(
        """
        2 + x3 * 4 // Hello world
                    // It's test
        + 4
        """)]
    public void AcceptsComments(string expression)
    {
        var parser = new SyntaxAnalyze.Analyzer(expression);
        Assert.That(parser.IsValidExpression(), Is.True);
        // Assert.That(SyntaxAnalyze.Analyzer.IsValidExpression(expression), Is.True);
    }

    [TestCase("((24 + 3) 4)")]
    [TestCase("(24 + 3")]
    [TestCase("24 + 3)")]
    [TestCase("24 + (3 & 6)")]
    [TestCase("24 + 3y,3")]
    [TestCase("24 + 1_2x + (x_*_y)")]
    [TestCase("24 + 12x + (x_*_y)")]
    public void ThrowsException(string expression)
    {
        var parser = new SyntaxAnalyze.Analyzer(expression);
        Assert.That(() => parser.IsValidExpression(), Throws.InvalidOperationException);
        //Assert.That(() => SyntaxAnalyze.Analyzer.IsValidExpression(expression), Throws.InvalidOperationException);
    }
}