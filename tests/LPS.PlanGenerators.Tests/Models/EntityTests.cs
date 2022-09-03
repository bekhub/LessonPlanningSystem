using FluentAssertions;
using LPS.PlanGenerators.Models;
using Xunit;

namespace LPS.PlanGenerators.Tests.Models;

public class EntityTests
{
    class TestModel : Model { }

    [Fact]
    public void TestEquality()
    {
        
    }
    
    [Fact]
    public void TestIfNull()
    {
        TestModel tst = null;
        (tst == null).Should().BeTrue();
        (tst != null).Should().BeFalse();
    }
}
