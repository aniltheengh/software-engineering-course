// === The same tests in xUnit ===
// Create the test project with:  dotnet new xunit -o Shop.Tests
// then reference the project under test and run:  dotnet test
using Xunit;

public class PriceCalculatorTests
{
    [Fact]   // one test case
    public void WithDiscount_TenPercent_ReducesPrice()
    {
        decimal result = PriceCalculator.WithDiscount(200m, 10);
        Assert.Equal(180m, result);
    }

    [Theory]   // one test method, many rows of data
    [InlineData(100, 0, 100)]
    [InlineData(100, 25, 75)]
    [InlineData(80, 50, 40)]
    [InlineData(100, 100, 0)]
    public void WithDiscount_ReturnsExpected(int price, int percent, int expected)
    {
        Assert.Equal(expected, PriceCalculator.WithDiscount(price, percent));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void WithDiscount_InvalidPercent_Throws(int percent)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PriceCalculator.WithDiscount(100m, percent));
    }
}

public class OrderServiceTests
{
    private readonly FakeClock _clock = new();          // xUnit creates a new instance per test,
    private readonly FakeEmailSender _email = new();    // so every test starts clean

    [Fact]
    public void Place_ValidOrder_SendsOneConfirmation()
    {
        var service = new OrderService(_clock, _email);

        Order order = service.Place("aiko@example.com", 99m);

        Assert.Equal(_clock.Now, order.PlacedAt);
        Assert.Single(_email.Sent);
        Assert.Contains("99", _email.Sent[0].Subject);
    }

    [Fact]
    public void Place_ZeroTotal_ThrowsAndSendsNothing()
    {
        var service = new OrderService(_clock, _email);

        Assert.Throws<ArgumentException>(() => service.Place("aiko@example.com", 0m));
        Assert.Empty(_email.Sent);
    }
}
