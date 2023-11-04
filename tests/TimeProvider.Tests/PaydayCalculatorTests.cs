namespace TimeProvider.Tests;

public class PaydayCalculatorTests
{
    [Fact]
    public void It_Is_Not_Payday()
    {
        var calculator = new PaydayCalculator();
        var actual = calculator.IsItPayday();
        Assert.False(actual, "It is payday when it shouldn't be 🤑");
    }

    [Fact]
    public void It_Is_Payday()
    {
        var calculator = new PaydayCalculator();
        var actual = calculator.IsItPayday();
        Assert.True(actual, "It is not payday 😢");
    }
}
