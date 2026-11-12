# 11 — Testing and code quality

**Duration:** 3 weeks · **Prerequisites:** 03

**Exercises:** [12 exercises for this module](../exercises/11-testing-and-code-quality.md)

## Goals
- Write code that is testable, and tests that are worth keeping
- Refactor safely

## Topics
1. Unit, integration and end-to-end tests; the test pyramid
2. xUnit: facts, theories, fixtures
3. Test doubles: stubs, mocks, fakes
4. Test-driven development
5. Code coverage and its limits
6. Refactoring and code smells
7. Static analysis, formatting, code review
8. Logging and diagnostics

## Code examples

All of the C# below is one runnable project in [examples/11-testing-and-code-quality](../examples/11-testing-and-code-quality). Run it with `dotnet run` from that folder, then change it and run it again.

### Code that is easy to test

```csharp
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
```

### Test doubles: fakes that stand in for real dependencies

```csharp
public class FakeClock : IClock
{
    public DateTime Now { get; set; } = new DateTime(2026, 1, 15, 9, 0, 0);
}

public class FakeEmailSender : IEmailSender
{
    public List<(string To, string Subject)> Sent { get; } = new();
    public void Send(string to, string subject) => Sent.Add((to, subject));
}
```

### Arrange, act, assert

```csharp
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
```

### The same tests in xUnit

From [`snippets/01-XunitTests.cs`](../examples/11-testing-and-code-quality/snippets/01-XunitTests.cs).

```csharp
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
```

## Resources
- **Primary:** [Unit testing C# with xUnit and dotnet test](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-csharp-with-xunit)
- [Unit testing best practices for .NET](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [xUnit.net documentation](https://xunit.net/)
- [Refactoring Guru — Refactoring](https://refactoring.guru/refactoring)
- Books: *Refactoring* (Fowler), *Unit Testing Principles, Practices, and Patterns* (Khorikov)

## Practice
- Add xUnit tests to the Module 04 collections library, aiming for meaningful coverage
- Do the Bowling Game and String Calculator katas with strict TDD
- Take the Gilded Rose kata: add tests first, then refactor
- Enable .NET analyzers and `dotnet format`; fix every warning

## Checkpoint project
Take one earlier project to **production quality**: test suite, analyzers clean, structured logging, and a short document of the refactorings you made.
