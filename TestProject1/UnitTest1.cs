namespace TestProject1;

using Calculator;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test, Category("Positive scenario")]
    public void ComputesExpressionWithSingleNum()
    {
        string expression = "2";
        double actual = Calculator.Compute(expression);
        double expected = 2;
        
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test, Category("Positive scenario")]
    public void ComputesBaseSum()
    {
        string expression = "2+3";
        double actual = Calculator.Compute(expression);
        double expected = 5;
        
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test, Category("Positive scenario")]
    public void ComputesWithPriority()
    {
        string expression = "1+2*3";
        double actual = Calculator.Compute(expression);
        double expected = 7;
        
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [Test, Category("Positive scenario")]
    public void ComputeMultiplicationFirst()
    {
        string expression = "2*4-4";
        double actual = Calculator.Compute(expression);
        double expected = 4;
        
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [Test, Category("Positive scenario")]
    public void ComputesWithSpaces()
    {
        string expression = "2 * 4 - 5";
        double actual = Calculator.Compute(expression);
        double expected = 3;
        
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [TestCase("2+3", 5)]
    [TestCase("1+2*3", 7)]
    [TestCase("2*4-4", 4)]
    [TestCase("2 * 4 - 5", 3)]
    [TestCase("(2 * 4) * 4 - 5 * (4 - 1)", 17)]
    [TestCase("2 * (2 + 3)", 10)]
    [TestCase("2 + (1 + 2 * 3)", 9)]
    [TestCase("2 * (1 + 3)", 8)]
    [TestCase("4 - (2 + 3 * 5)", -13)]
    [TestCase("4 - ((2 + 12) / (3 + 4))", 2)]
    [Category("Positive scenario")]
    public void Computes(string expression, double expected)
    {
        double actual = Calculator.Compute(expression);
        
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [Test, Category("Negative scenario")]
    public void ThrowsWithInvalidOperator()
    {
        string expression = "2 & 4 - 5";
       
        Assert.Throws<InvalidOperationException>(() => Calculator.Compute(expression), "Operators must be +, -, * or / only."); 
    }
    
    [Test, Category("Negative scenario")]
    public void ThrowsWithDivisionByZero()
    {
        string expression = "2 / 0 - 5";
       
        Assert.Throws<DivideByZeroException>(() => Calculator.Compute(expression), "Division by zero is not allowed."); 
    }
}