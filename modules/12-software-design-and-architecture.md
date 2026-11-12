# 12 — Software design and architecture

**Duration:** 4 weeks · **Prerequisites:** 03, 11

**Exercises:** [14 exercises for this module](../exercises/12-software-design-and-architecture.md)

## Goals
- Structure a medium-sized codebase so it stays easy to change
- Know the common patterns and, more importantly, when not to use them

## Topics
1. Coupling, cohesion, information hiding
2. SOLID principles
3. Design patterns: strategy, factory, observer, decorator, adapter, command, builder
4. Dependency injection and inversion of control
5. Layered, hexagonal and clean architecture
6. Domain modelling basics
7. API design and versioning
8. Requirements, UML sketches, architecture decision records

## Code examples

All of the C# below is one runnable project in [examples/12-software-design-and-architecture](../examples/12-software-design-and-architecture). Run it with `dotnet run` from that folder, then change it and run it again.

### Strategy: swap an algorithm without editing the caller (open/closed)

```csharp
interface IShippingStrategy { decimal Cost(decimal weightKg); }

class StandardShipping : IShippingStrategy { public decimal Cost(decimal weightKg) => 5m + 1.0m * weightKg; }
class ExpressShipping : IShippingStrategy { public decimal Cost(decimal weightKg) => 12m + 2.5m * weightKg; }
class FreeShipping : IShippingStrategy { public decimal Cost(decimal weightKg) => 0m; }

class Checkout
{
    private readonly IShippingStrategy _shipping;
    public Checkout(IShippingStrategy shipping) => _shipping = shipping;
    public decimal Total(decimal goods, decimal weightKg) => goods + _shipping.Cost(weightKg);
}

static class StrategyDemo
{
    public static void Run()
    {
        // A new shipping rule is a new class. Checkout never changes.
        Console.WriteLine(new Checkout(new StandardShipping()).Total(100m, 4m));   // 109.0
        Console.WriteLine(new Checkout(new ExpressShipping()).Total(100m, 4m));    // 122.0
        Console.WriteLine(new Checkout(new FreeShipping()).Total(100m, 4m));       // 100
    }
}
```

### Dependency injection: depend on abstractions, wire them at the edge

```csharp
interface ILogger { void Log(string message); }
interface IReportStore { void Save(string name, string content); }

class ConsoleLogger : ILogger { public void Log(string message) => Console.WriteLine($"[log] {message}"); }

class MemoryReportStore : IReportStore
{
    public Dictionary<string, string> Files { get; } = new();
    public void Save(string name, string content) => Files[name] = content;
}

class ReportService
{
    private readonly ILogger _logger;
    private readonly IReportStore _store;

    // The class asks for what it needs; it never calls "new" on a concrete dependency.
    public ReportService(ILogger logger, IReportStore store)
    {
        _logger = logger;
        _store = store;
    }

    public void Generate(string name, IEnumerable<int> values)
    {
        _store.Save(name, $"count={values.Count()}, sum={values.Sum()}");
        _logger.Log($"report '{name}' saved");
    }
}

static class DependencyInjectionDemo
{
    public static void Run()
    {
        // The "composition root": the one place that knows the concrete classes.
        var store = new MemoryReportStore();
        var service = new ReportService(new ConsoleLogger(), store);

        service.Generate("flows", new[] { 3, 5, 8 });
        Console.WriteLine(store.Files["flows"]);   // count=3, sum=16
    }
}
```

### Factory: one place decides which class to create

```csharp
interface IShape { double Area { get; } }
record Circle(double Radius) : IShape { public double Area => Math.PI * Radius * Radius; }
record Rect(double Width, double Height) : IShape { public double Area => Width * Height; }

static class ShapeFactory
{
    public static IShape Create(string spec)
    {
        string[] parts = spec.Split(' ');
        return parts[0] switch
        {
            "circle" => new Circle(double.Parse(parts[1])),
            "rect" => new Rect(double.Parse(parts[1]), double.Parse(parts[2])),
            _ => throw new ArgumentException($"Unknown shape '{parts[0]}'")
        };
    }
}

static class FactoryDemo
{
    public static void Run()
    {
        foreach (string spec in new[] { "circle 1", "rect 2 3" })
            Console.WriteLine($"{spec}: area {ShapeFactory.Create(spec).Area:F2}");   // 3.14, 6.00
    }
}
```

### Observer: publish changes to whoever is listening

```csharp
interface IObserver<in T> { void Update(T value); }

class Subject<T>
{
    private readonly List<IObserver<T>> _observers = new();
    public void Attach(IObserver<T> observer) => _observers.Add(observer);
    public void Detach(IObserver<T> observer) => _observers.Remove(observer);
    public void Publish(T value) { foreach (var o in _observers.ToList()) o.Update(value); }
}

class Display : IObserver<double> { public void Update(double value) => Console.WriteLine($"display: {value} bar"); }
class Alarm : IObserver<double> { public void Update(double value) { if (value > 10) Console.WriteLine("alarm: over pressure"); } }

static class ObserverDemo
{
    public static void Run()
    {
        var pressure = new Subject<double>();
        var display = new Display();
        pressure.Attach(display);
        pressure.Attach(new Alarm());

        pressure.Publish(6.5);    // display only
        pressure.Publish(11.2);   // display and alarm
        pressure.Detach(display);
        pressure.Publish(12.0);   // alarm only
    }
}
```

### Decorator: add behaviour by wrapping, not by editing

```csharp
interface ITextSource { string Read(); }

class FileLikeSource : ITextSource { public string Read() => "  raw text from somewhere  "; }

class TrimmingSource : ITextSource
{
    private readonly ITextSource _inner;
    public TrimmingSource(ITextSource inner) => _inner = inner;
    public string Read() => _inner.Read().Trim();
}

class UpperCaseSource : ITextSource
{
    private readonly ITextSource _inner;
    public UpperCaseSource(ITextSource inner) => _inner = inner;
    public string Read() => _inner.Read().ToUpperInvariant();
}

class CachingSource : ITextSource
{
    private readonly ITextSource _inner;
    private string? _cached;
    public int InnerReads { get; private set; }
    public CachingSource(ITextSource inner) => _inner = inner;

    public string Read()
    {
        if (_cached == null) { _cached = _inner.Read(); InnerReads++; }
        return _cached;
    }
}

static class DecoratorDemo
{
    public static void Run()
    {
        var cached = new CachingSource(new UpperCaseSource(new TrimmingSource(new FileLikeSource())));
        Console.WriteLine($"[{cached.Read()}]");   // [RAW TEXT FROM SOMEWHERE]
        cached.Read();
        cached.Read();
        Console.WriteLine($"inner source was read {cached.InnerReads} time(s)");   // 1
    }
}
```

### Adapter: make an old interface fit the one you want

```csharp
class LegacyPrinter   // imagine this is a library you cannot change
{
    public void PrintLine(string text, int copies) { for (int i = 0; i < copies; i++) Console.WriteLine($"<legacy> {text}"); }
}

interface IPrinter { void Print(string text); }

class LegacyPrinterAdapter : IPrinter
{
    private readonly LegacyPrinter _legacy = new();
    public void Print(string text) => _legacy.PrintLine(text, copies: 1);
}

static class AdapterDemo
{
    public static void Run()
    {
        IPrinter printer = new LegacyPrinterAdapter();   // the rest of the code sees only IPrinter
        printer.Print("drawing A-101");
    }
}
```

## Resources
- **Primary:** [Refactoring Guru — Design Patterns](https://refactoring.guru/design-patterns) (C# examples)
- [.NET application architecture guides](https://learn.microsoft.com/en-us/dotnet/architecture/)
- [Dependency injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- Books: *A Philosophy of Software Design* (Ousterhout), *Head First Design Patterns*, *Clean Architecture* (Martin)

## Practice
- Implement seven patterns above, each solving a concrete problem rather than a toy
- Find a SOLID violation in your old code for each principle and fix it
- Rewire a project to use `Microsoft.Extensions.DependencyInjection`
- Write an architecture decision record for a real choice you made

## Checkpoint project
A **plugin-based application**: a host that discovers and loads plugins through an interface (for example a file-conversion or report-generation tool), with a design document.
