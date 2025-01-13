using Pdf2Html.Settings;

namespace Unit.Tests;

public class ConversionOptionsTests
{
    [Test]
    public void TestToCommandLineArguments()
    {
        var input = new Dictionary<string, string?>
        {
            { "ConversionOptions:FooBar", "true" },
            { "ConversionOptions:BazBlort", "FALSE" },
            { "ConversionOptions:Hello", "World!" },
            { "ConversionOptions:FizzBuzz", "5" },
        };
        var result = ConversionOptions.ToCommandLineArguments(input);
        Assert.That(result, Is.EqualTo("--foo-bar=1 --baz-blort=0 --hello=World! --fizz-buzz=5"));
    }
}
