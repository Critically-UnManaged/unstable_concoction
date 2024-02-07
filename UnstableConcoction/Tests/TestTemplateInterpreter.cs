using GdUnit4;
using Serilog;

namespace UnstableConcoction.Tests;
using static Assertions;

[TestSuite]
public class TestTemplateInterpreter
{
    
    [TestCase]
    public void TestTestCase()
    {
        AssertThat(true).IsTrue();
    }
}