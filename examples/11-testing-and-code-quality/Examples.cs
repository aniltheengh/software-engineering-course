// Run with: dotnet run
// The code under test plus a tiny hand-made test runner, so the ideas are visible
// without a framework. The xUnit versions of the same tests are in the snippets folder.
MiniTests.Run();

// === Code that is easy to test ===
// Pure function: same input, same output, no hidden dependencies.
public static class PriceCalculator
{
    public static decimal WithDiscount(decimal price, int percent)
    {
        if (percent < 0 || percent > 100) throw new ArgumentOutOfRangeException(nameof(percent));
        return Math.Round(price * (100 - percent) / 100m, 2);
    }
}

// Dependencies come in through interfaces, so a test can replace them.
public interface IClock { DateTime Now { get; } }
public interface IEmailSender { void Send(string to, string subject); }

public record Order(string CustomerEmail, decimal Total, DateTime PlacedAt);

public class OrderService
{
    private readonly IClock _clock;
    private readonly IEmailSender _email;

    public OrderService(IClock clock, IEmailSender email)
    {
        _clock = clock;
        _email = email;
    }

    public Order Place(string customerEmail, decimal total)
    {
        if (total <= 0) throw new ArgumentException("Total must be positive", nameof(total));
        var order = new Order(customerEmail, total, _clock.Now);
        _email.Send(customerEmail, $"Order confirmed: {total}");
        return order;
    }
}

// === Test doubles: fakes that stand in for real dependencies ===
public class FakeClock : IClock
{
    public DateTime Now { get; set; } = new DateTime(2026, 1, 15, 9, 0, 0);
}

public class FakeEmailSender : IEmailSender
{
    public List<(string To, string Subject)> Sent { get; } = new();
    public void Send(string to, string subject) => Sent.Add((to, subject));
}

// === Arrange, act, assert ===
static class MiniTests
{
    static int passed, failed;

    static void Test(string name, Action body)
    {
        try { body(); passed++; Console.WriteLine($"PASS  {name}"); }
        catch (Exception ex) { failed++; Console.WriteLine($"FAIL  {name}: {ex.Message}"); }
    }

    static void AssertEqual<T>(T expected, T actual)
    {
        if (!Equals(expected, actual)) throw new Exception($"expected {expected} but got {actual}");
    }

    public static void Run()
    {
        Test("discount of 10 percent", () =>
        {
            decimal result = PriceCalculator.WithDiscount(200m, 10);   // act
            AssertEqual(180m, result);                                 // assert
        });

        Test("discount above 100 is rejected", () =>
        {
            bool threw = false;
            try { PriceCalculator.WithDiscount(200m, 101); }
            catch (ArgumentOutOfRangeException) { threw = true; }
            AssertEqual(true, threw);
        });

        Test("placing an order stamps the time and sends one email", () =>
        {
            var clock = new FakeClock();                               // arrange
            var email = new FakeEmailSender();
            var service = new OrderService(clock, email);

            Order order = service.Place("aiko@example.com", 99m);      // act

            AssertEqual(clock.Now, order.PlacedAt);                    // assert
            AssertEqual(1, email.Sent.Count);
            AssertEqual("aiko@example.com", email.Sent[0].To);
        });

        Console.WriteLine($"{passed} passed, {failed} failed");
    }
}
