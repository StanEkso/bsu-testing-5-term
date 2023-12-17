namespace TestCalculator;

using Execution.Compiled;
using Execution;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [TestCase(".5", .5)]
    [TestCase("5.0", 5)]
    [TestCase("5.0e0", 5)]
    [TestCase("5.0e-0", 5)]
    [TestCase("5.0e+0", 5)]
    [TestCase("50e-1", 5)]
    [TestCase("500E-2", 5)]
    [TestCase("0.500E1", 5)]
    [TestCase("0.500E+1", 5)]
    [TestCase("0.5e+1", 5)]
    [TestCase(".5e+1", 5)]
    [TestCase("2.0+3.0", 5)]
    [TestCase("2+3.0", 5)]
    [TestCase("2.0+3", 5)]
    [TestCase("2.1+2.9", 5)]
    [TestCase("  2.1    +   2.9 ", 5)]
    [TestCase("7.0 - .4 - .6 ", 6)]
    [TestCase("7.0 - 2", 5)]
    [TestCase("7 - 2.0", 5)]
    [TestCase("7.0 - 2.0", 5)]
    [TestCase(" - 2.0 + 7.0", 5)]
    [TestCase(" -2.0 + 7.000", 5)]
    [TestCase(" 1/3.0 * 5.0*3 ", 5)]
    [TestCase(" 5/3.0 * 3.0 ", 5)]
    [TestCase(" 1/3.0 * 5 * 3 ", 5)]
    [TestCase(" 1.0/3 * 5 * 3 ", 5)]
    [TestCase(" (1/3.0) * (5 * 3) ", 5)]
    [TestCase(" -(1/3.0) * (-5 * 3) ", 5)]
    [Test, Category("Positive scenario")]
    public void ComputesDoubleExpression(string expression, double expected)
    {
        TypedValue? actual = Execution.CalcExpression(expression); //Calculator.Compute(expression);
        var actualDouble = actual?.doubleValue;
        //Token actual = Execution.CalcExpression(expression); //Calculator.Compute(expression);
        //var actualDouble = (actual as TokenConstant<double>).value;

        const double tolerance = 1e-100;

        Assert.That(AlmostEquals((actualDouble ?? double.NaN), expected, tolerance));
        // Assert.That(Math.Abs((actualDouble ?? double.NaN) / expected - 1), Is.LessThanOrEqualTo(tolerance));
    }

    [TestCase(" 2 +3 ", 5)]
    [TestCase(" +2 +3 ", 5)]
    [TestCase("7 - 2", 5)]
    [TestCase(" - 2 + 7", 5)]
    [TestCase(" -20 + 25  ", 5)]
    [TestCase(" (5/1) * (3 / 3) ", 5)]
    [TestCase(" (50/10)", 5)]
    [TestCase(" -(50/10)*(-1)", 5)]
    [TestCase(" -(50/10)*(-1)/(-1)", -5)]
    [TestCase(" -(50/10 + 5*2*3)*(-1)/(-1)", -35)]
    [TestCase(" -(50/10 + 5*2*3*0)*(-1)/(-1)", -5)]
    [TestCase(" -(50/10 + 5*2*3*0)*(-1)/(-1) + 2 - 2 + 0*4*5 ", -5)]
    [Test, Category("Positive scenario")]
    public void ComputesIntExpression(string expression, int expected)
    {
        TypedValue? actual = Execution.CalcExpression(expression); //Calculator.Compute(expression);
        var actualInt = actual?.intValue;
        //Token actual = Execution.CalcExpression(expression); //Calculator.Compute(expression);
        //var actualInt = (actual as TokenConstant<int>).value;

        Assert.That(actualInt, Is.EqualTo(expected));
    }

    [TestCase("var x = 1;", 0)]
    [TestCase("return;", 0)]
    [TestCase("return 1;", 1)]
    [TestCase("return 1+1;", 2)]
    [TestCase("return -1+2;", 1)]
    [TestCase("return -1+(2*30*5) +1;", 300)]
    [TestCase("return -(1+2);", -3)]
    [TestCase("return 1 + 10*3 + 9;", 40)]
    [TestCase("return 1 + (10*3 + 9) / 13 ;", 4)]
    [TestCase("""
                if (2==2) { return 1; }
              """, 1)]
    [TestCase("""
                if (2==0) { return 1; }
              """, 0)]
    [TestCase("""
                if (11 > 0) { return 1; }
              """, 1)]
    [TestCase("""
                if (11 > 0) { return 1; }
                else { return 22; }
              """, 1)]
    [TestCase("""
                if (11 < 0) { return 1; }
                else { return 22; }
              """, 22)]
    [TestCase("""
              var x = 1;
              if x == 1 { return 100; }
                else { return 500; }
              """, 100)]
    [TestCase("""
              var x = -1;
              if (x == -1) { return 100; }
                else { return 500; }
              """, 100)]
    [TestCase("""
              var x = 2;
              if (x != 1) { return 100; }
                else { return 500; }
              """, 100)]
    [TestCase("""
              var x = 2;
              if x > 1 { return 100; }
                else { return 500; }
              """, 100)]
    [TestCase("""
              var x = 2;
              if x > 10 { return 100; }
                else if (x > 100) { return 500; }
                else if (x < 5)   { return 15; }
                else  { return 99; }
              """, 15)]
    [TestCase("""
              var i = 2;
              return i; 
              """, 2)]
    [TestCase("""
              var i = 0;
              while i < 10 
              {
                i = i + 1; 
              }
              return i; 
              """, 10)]
    [TestCase("""
              var i = 0;
              var j = 2;
              while i < 10 && j <= 2 
              {
                i = i + 1;
                j = i % 3;
              }
              return i+0; 
              """, 10)]
    [TestCase("""
              var i = 0;
              while (i < 10)
              {
                i = i + 1 - 2*0; 
              }
              return i*100; 
              """, 1000)]
    [TestCase("""
              var i = 0;
              while (i < 100)
              {
                i = i + 1 - 2*0; 
                if i == 20 { 
                    return; 
                }
              }
              return 100; 
              """, 0)]
    [TestCase("""
              var i = 0;
              while (i < 100)
              {
                i = i + 1 - 2*0; 
                if i % 50 > 40 + 1.0 { 
                    return i % 50; 
                }
              }
              return 100; 
              """, 42)]
    public void TestExecInt(string expression, int expected)
    {
        TypedValue? actual = Execution.Exec(expression);        
        var actualInt = actual?.intValue;                   
        //Token? actual = Execution.Exec(expression); //Calculator.Compute(expression);
        //var actualInt = (actual as TokenConstant<int>)?.value;

        Assert.That(actualInt, Is.EqualTo(expected));
    }

    [TestCase("""
              function f()
              {
                return 5; 
              }
              return 100; 
              """, 100)]

    [TestCase("""
              var x = 11, y=22;
              var z = 333;
              function f1()
              {
                x = 1;
                return 5; 
              }
              function f2()
              {
                x = 2;
                return 15; 
              }
              return x + y + z; 
              """, 366)]
    [TestCase("""
              var x = 7;
              function f1()
              {
                return 5; 
              }
              x = f1() + f1();
              return x; 
              """, 10)]
    [TestCase("""
              var x = 7;
              function f1()
              {
                return 5; 
              }
              x = f1() + f1()*f1();
              return x; 
              """, 30)]
    [TestCase("""
              var x = 7;
              function f1()
              {
                return 5; 
              }
              x = f1() * f1() - f1();
              return x; 
              """, 20)]
    [TestCase("""
              var x = 7;
              function f1()
              {
                x = 1;
                return 5; 
              }
              x = f1();
              return x; 
              """, 5)]
    [TestCase("""
              var x = 11;
              function f1()
              {
                x = 1;
                return 5; 
              }
              return x + f1(); // !! 1:f1(x=1) = 5    2:5 + x = 6
              """, 16)]
    [TestCase("""
              var x = 11;
              function f1()
              {
                x = 1;
                return 5; 
              }
              return (x) + f1(); // !! (x)  - x is TokenVar: ref, not value
              """, 16)]
    [TestCase("""
              var x = 11;
              function f1()
              {
                x = 1;
                return 5; 
              }
              return (x+0) + f1(); // !! (x+0)  - now value
              """, 16)]
    [TestCase("""
              var x = 11;
              function f1()
              {
                x = 1;
                return 5; 
              }
              return (x*1) + f1(); // !! 1:(x*1) = 11  2:f1()  3:5 + 11 = 16
              """, 16)]
    public void TestExecFunc(string expression, int expected)
    {
        TypedValue? actual = Execution.Exec(expression);
        var actualInt = actual?.intValue;

        Assert.That(actualInt, Is.EqualTo(expected));
    }


    [TestCase("""
              function f(x, y)
              {
                return x + y; 
              }
              return f(20, 30);
              """, 50)]
    public void TestExecFuncParam(string expression, int expected)
    {
        TypedValue? actual = Execution.Exec(expression);
        var actualInt = actual?.intValue;

        Assert.That(actualInt, Is.EqualTo(expected));
    }


    [TestCase("""
              var i = 2;
              var j = i + i * 1.0; // 4.0
              var k = i * j;       // 8.0
              k = -k / (-k) * -(0-k);
              return k; // 8
              """, 8)]
    [TestCase("""
              var k = 8.0;
              if k != 8 {
                k = 11;
              }
              if -k != -8 {
                k = 12;
              }
              return k; // 8
              """, 8)]
    public void TestExecDouble(string expression, double expected)
    {
        TypedValue? actual = Execution.Exec(expression); //Calculator.Compute(expression);
        var actualDouble = actual?.doubleValue;

        const double tolerance = 1e-100;

        // both absolute and relative diffs
        Assert.That(AlmostEquals((actualDouble ?? double.NaN), expected, tolerance));

        // only absolute diff
        // Assert.That(Math.Abs((actualDouble ?? double.NaN - expected), Is.LessThanOrEqualTo(tolerance));

        // only relative diff
        // Assert.That(Math.Abs((actualDouble ?? double.NaN) / expected - 1), Is.LessThanOrEqualTo(tolerance));
    }

    [TestCase("""
              var str = 'this is string';
              return str;
              """, "this is string")]
    [TestCase("""
              var str = 'this is string';
              if str != 'this is string' {
                str = 'wtf?';
              }
              return str;
              """, "this is string")]
    [TestCase("""
              var str1 = 'string1';
              var str2 = 'string2';
              var str3;
              if str1 != str2 {
                str3 = 'wtf?';
              }
              return str1 + str2;
              """, "string1string2")]
    [TestCase("""
              var str1 = 'string';
              var str2 = 'string2';
              if str2 == str1 + '2' {
                return 'ok2';
              }
              return str1 + str2;
              """, "ok2")]
    [TestCase("""
              var str1 = 'string';
              var str2 = 'string2';
              if str2 + '34' == str1 + '2' + '34' {
                return 'ok234';
              }
              return str1 + str2;
              """, "ok234")]
    [TestCase("""
              var str1 = 'string1';
              var str2 = 'string2';
              return str1 + ' ' + str2;
              """, "string1 string2")]
    public void TestExecString(string expression, string expected)
    {
        TypedValue? actual = Execution.Exec(expression); //Calculator.Compute(expression);
        var actualString = actual?.stringValue;

        Assert.That(actualString, Is.EqualTo(expected));
    }

    public static bool AlmostEquals(double x, double y, double tolerance)
    {
    // https://roundwide.com/equality-comparison-of-floating-point-numbers-in-csharp/

        var diff = Math.Abs(x - y);
        return diff <= tolerance ||
               diff <= Math.Max(Math.Abs(x), Math.Abs(y)) * tolerance;
    }
    
    //[Test, Category("Positive scenario")]
    //public void ComputesWithPriority()
    //{
    //    string expression = "1+2*3";
    //    double actual = Execution.Exec(expression); //Calculator.Compute(expression);
    //    double expected = 7;

    //    Assert.That(actual, Is.EqualTo(expected));
    //}

    //[Test, Category("Positive scenario")]
    //public void ComputeMultiplicationFirst()
    //{
    //    string expression = "2*4-4";
    //    double actual = Execution.Exec(expression); //Calculator.Compute(expression);
    //    double expected = 4;

    //    Assert.That(actual, Is.EqualTo(expected));
    //}

    //[Test, Category("Positive scenario")]
    //public void ComputesWithSpaces()
    //{
    //    string expression = "2 * 4 - 5";
    //    double actual = Execution.Exec(expression); //Calculator.Compute(expression);
    //    double expected = 3;

    //    Assert.That(actual, Is.EqualTo(expected));
    //}

    //[TestCase("2+3", 5)]
    //[TestCase("1+2*3", 7)]
    //[TestCase("2*4-4", 4)]
    //[TestCase("2 * 4 - 5", 3)]
    //[TestCase("(2 * 4) * 4 - 5 * (4 - 1)", 17)]
    //[TestCase("2 * (2 + 3)", 10)]
    //[TestCase("2 + (1 + 2 * 3)", 9)]
    //[TestCase("2 * (1 + 3)", 8)]
    //[TestCase("4 - (2 + 3 * 5)", -13)]
    //[TestCase("4 - ((2 + 12) / (3 + 4))", 2)]
    //[Category("Positive scenario")]
    //public void Computes(string expression, double expected)
    //{
    //    double actual = Execution.Exec(expression); //Calculator.Compute(expression);

    //    Assert.That(actual, Is.EqualTo(expected));
    //}


    ////[TestCase("2 * (1 + 3) *    ")]
    ////[TestCase("4 - (2 + 3 * 5   ")]
    ////[Test, Category("Negative scenario")]
    ////public void CalculatorReturnNaN(string expression)
    ////{
    ////    double actual = Execution.Exec(expression); //Calculator.Compute(expression);
    ////    Assert.That(actual, Is.EqualTo(double.NaN));
    ////}

    //[TestCase("2 * (1 + 3) *   ")]
    //[TestCase("4 - (2 + 3 * 5  ")]
    //[TestCase("2 & 4 - 5       ")]
    //[TestCase("+ - * /         ")]
    ////[TestCase("1 2 +           ")]
    //[TestCase("1 + ( ) *)      ")]
    //[Test, Category("Negative scenario")]
    //public void ThrowsWithInvalidOperator(string expression)
    //{
    // Assert.Throws<InvalidOperationException>(() =>
    //            Execution.Exec(expression)  //Calculator.Compute(expression)
    //              , "Operators must be +, -, * or / only."); 
    //}

    //[Test, Category("Negative scenario")]
    //public void ThrowsWithDivisionByZero()
    //{
    //    string expression = "2 / 0 - 5";

    //    Assert.Throws<DivideByZeroException>(() =>
    //            Execution.Exec(expression)  //Calculator.Compute(expression)
    //                , "Division by zero is not allowed."); 
    //}
}