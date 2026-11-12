# 03 — Object-oriented programming

**Duration:** 4 weeks · **Prerequisites:** 01–02

**Exercises:** [12 exercises for this module](../exercises/03-object-oriented-programming.md)

## Goals
- Model a problem domain with classes, interfaces and records
- Explain when to use inheritance and when composition is better

## Topics
1. Classes, objects, constructors, properties, access modifiers
2. Encapsulation and invariants
3. Inheritance, `virtual`/`override`, abstract classes
4. Interfaces and polymorphism
5. Composition over inheritance
6. Generics and constraints
7. Records, structs, enums, immutability
8. Delegates, lambdas, events
9. `IDisposable` and `using`

## Code examples

All of the C# below is one runnable project in [examples/03-object-oriented-programming](../examples/03-object-oriented-programming). Run it with `dotnet run` from that folder, then change it and run it again.

### Encapsulation: a class protects its own rules

```csharp
class Tank
{
    public string Tag { get; }                  // set once, in the constructor
    public double CapacityLitres { get; }
    public double LevelLitres { get; private set; }   // readable by all, changed only here

    public Tank(string tag, double capacityLitres)
    {
        if (capacityLitres <= 0) throw new ArgumentOutOfRangeException(nameof(capacityLitres));
        Tag = tag;
        CapacityLitres = capacityLitres;
    }

    public void Fill(double litres)
    {
        if (litres < 0) throw new ArgumentOutOfRangeException(nameof(litres));
        if (LevelLitres + litres > CapacityLitres) throw new InvalidOperationException("Tank would overflow");
        LevelLitres += litres;
    }

    public double PercentFull => LevelLitres / CapacityLitres * 100;   // computed property
}

static class EncapsulationDemo
{
    public static void Run()
    {
        var tank = new Tank("T-100", 500);
        tank.Fill(125);
        Console.WriteLine($"{tank.Tag}: {tank.PercentFull}% full");   // T-100: 25% full
        // tank.LevelLitres = 9999;   // does not compile: the setter is private
        try { tank.Fill(1000); }
        catch (InvalidOperationException ex) { Console.WriteLine(ex.Message); }
    }
}
```

### Inheritance and polymorphism

```csharp
abstract class Valve
{
    public string Tag { get; }
    protected Valve(string tag) => Tag = tag;

    public abstract double PressureDrop(double flow);        // each subclass must provide this
    public virtual string Describe() => $"{Tag} ({GetType().Name})";   // may be overridden
}

class GateValve : Valve
{
    public GateValve(string tag) : base(tag) { }
    public override double PressureDrop(double flow) => 0.1 * flow;
}

class GlobeValve : Valve
{
    public GlobeValve(string tag) : base(tag) { }
    public override double PressureDrop(double flow) => 0.8 * flow;
    public override string Describe() => base.Describe() + " - throttling";
}

static class PolymorphismDemo
{
    public static void Run()
    {
        var valves = new List<Valve> { new GateValve("V-1"), new GlobeValve("V-2") };
        foreach (Valve v in valves)   // one loop, the right method is chosen at run time
            Console.WriteLine($"{v.Describe()}: {v.PressureDrop(10)} kPa");
    }
}
```

### Interfaces: a contract without an implementation

```csharp
interface IExporter
{
    string Export(IEnumerable<string> rows);
}

class CsvExporter : IExporter
{
    public string Export(IEnumerable<string> rows) => string.Join(",", rows);
}

class MarkdownExporter : IExporter
{
    public string Export(IEnumerable<string> rows) => "| " + string.Join(" | ", rows) + " |";
}

static class InterfaceDemo
{
    // This method works with any exporter, including ones written later.
    static void Print(IExporter exporter) => Console.WriteLine(exporter.Export(new[] { "DN50", "PN16", "SUS304" }));

    public static void Run()
    {
        Print(new CsvExporter());        // DN50,PN16,SUS304
        Print(new MarkdownExporter());   // | DN50 | PN16 | SUS304 |
    }
}
```

### Composition: build objects out of other objects

```csharp
class Motor
{
    public double PowerKw { get; }
    public Motor(double powerKw) => PowerKw = powerKw;
}

class Pump
{
    private readonly Motor _motor;          // a pump HAS a motor; it is not a kind of motor
    public string Tag { get; }

    public Pump(string tag, Motor motor)
    {
        Tag = tag;
        _motor = motor;
    }

    public string Summary => $"{Tag} driven by a {_motor.PowerKw} kW motor";
}

static class CompositionDemo
{
    public static void Run() => Console.WriteLine(new Pump("P-101", new Motor(7.5)).Summary);
}
```

### Generics: one implementation for many types

```csharp
class Inventory<T>
{
    private readonly List<T> _items = new();
    public void Add(T item) => _items.Add(item);
    public int Count => _items.Count;
    public T Latest => _items[^1];   // ^1 means "last element"
}

static class GenericsDemo
{
    // The constraint says: T must be comparable with itself.
    static T Largest<T>(IEnumerable<T> items) where T : IComparable<T>
    {
        T best = items.First();
        foreach (T item in items)
            if (item.CompareTo(best) > 0) best = item;
        return best;
    }

    public static void Run()
    {
        var tags = new Inventory<string>();
        tags.Add("E-201");
        tags.Add("E-202");
        Console.WriteLine($"{tags.Count} tags, latest {tags.Latest}");   // 2 tags, latest E-202

        Console.WriteLine(Largest(new[] { 3, 17, 9 }));          // 17
        Console.WriteLine(Largest(new[] { "pear", "apple" }));   // pear
    }
}
```

### Records: immutable data with value equality

```csharp
record Point(double X, double Y);

static class RecordsDemo
{
    public static void Run()
    {
        var a = new Point(1, 2);
        var b = new Point(1, 2);
        Console.WriteLine(a == b);          // True: records compare by content
        var moved = a with { X = 10 };      // copy with one property changed
        Console.WriteLine(moved);           // Point { X = 10, Y = 2 }
        var (x, y) = moved;                 // deconstruction
        Console.WriteLine(x + y);           // 12
    }
}
```

### Delegates and events

```csharp
class LevelSensor
{
    public event Action<double>? LevelChanged;   // subscribers are methods taking a double

    public void Report(double level) => LevelChanged?.Invoke(level);   // ?. : no subscribers, no call
}

static class EventsDemo
{
    public static void Run()
    {
        Func<double, double> toPercent = litres => litres / 500 * 100;   // a delegate holding a lambda

        var sensor = new LevelSensor();
        Action<double> log = level => Console.WriteLine($"log: {toPercent(level)}%");
        sensor.LevelChanged += log;
        sensor.LevelChanged += level => { if (level > 450) Console.WriteLine("ALARM: high level"); };

        sensor.Report(250);   // log: 50%
        sensor.Report(480);   // log: 96%  then  ALARM: high level

        sensor.LevelChanged -= log;   // unsubscribe
        sensor.Report(100);           // prints nothing
    }
}
```

### IDisposable and using

```csharp
class Connection : IDisposable
{
    public Connection() => Console.WriteLine("connection opened");
    public void Send(string message) => Console.WriteLine($"sent: {message}");
    public void Dispose() => Console.WriteLine("connection closed");
}

static class DisposeDemo
{
    public static void Run()
    {
        using (var connection = new Connection())
        {
            connection.Send("hello");
        }   // Dispose runs here, even if Send had thrown
        Console.WriteLine("after the using block");
    }
}
```

## Resources
- **Primary:** [Object-oriented programming in C#](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/oop)
- [C# fundamentals: types, classes, interfaces, generics](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/)
- [Delegates and events](https://learn.microsoft.com/en-us/dotnet/csharp/delegates-overview)
- Book: *Head First C#* (Stellman & Greene) or *C# in Depth* (Jon Skeet) for a deeper pass

## Practice
- Shape hierarchy with area/perimeter, first by inheritance, then by interface
- Bank account model that makes invalid states impossible
- Generic `Result<T>` type and a generic repository interface
- Event-driven temperature monitor using delegates and events

## Checkpoint project
A **library management** or **parts catalogue** system: at least five classes, two interfaces, one generic type, persistence behind an interface so the storage can be swapped.
