// Run with: dotnet run
EncapsulationDemo.Run();
PolymorphismDemo.Run();
InterfaceDemo.Run();
CompositionDemo.Run();
GenericsDemo.Run();
RecordsDemo.Run();
EventsDemo.Run();
DisposeDemo.Run();

// === Encapsulation: a class protects its own rules ===
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

// === Inheritance and polymorphism ===
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

// === Interfaces: a contract without an implementation ===
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

// === Composition: build objects out of other objects ===
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

// === Generics: one implementation for many types ===
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

// === Records: immutable data with value equality ===
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

// === Delegates and events ===
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

// === IDisposable and using ===
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
