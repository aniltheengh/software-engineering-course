// Run with: dotnet run
Types.Run();
ControlFlow.Run();
Methods.Run();
Collections.Run();
ValueVsReference.Run();
ErrorsAndFiles.Run();
LinqBasics.Run();

// === Variables, types and conversion ===
static class Types
{
    public static void Run()
    {
        int count = 42;
        double flow = 12.5;        // floating point: fast, not exact
        decimal price = 19.99m;    // decimal: exact, use it for money
        bool isOpen = true;
        char grade = 'A';
        string name = "Pump P-101";
        var ratio = 0.75;          // var: the compiler infers double

        Console.WriteLine($"{name}: {flow:F2} m3/h, open={isOpen}, count={count}");
        Console.WriteLine($"price={price}, grade={grade}, ratio={ratio:P0}");

        Console.WriteLine(7 / 2);      // 3    integer division drops the remainder
        Console.WriteLine(7 / 2.0);    // 3.5
        Console.WriteLine(7 % 2);      // 1    remainder
        Console.WriteLine((int)3.99);  // 3    a cast truncates, it does not round

        // Never trust text input: TryParse returns false instead of throwing.
        if (int.TryParse("123", out int parsed)) Console.WriteLine(parsed + 1);   // 124
        Console.WriteLine(int.TryParse("12x", out _));                            // False
    }
}

// === Control flow ===
static class ControlFlow
{
    public static void Run()
    {
        int temperature = 68;

        if (temperature > 80) Console.WriteLine("too hot");
        else if (temperature >= 60) Console.WriteLine("normal");
        else Console.WriteLine("too cold");

        // A switch expression returns a value.
        string label = temperature switch
        {
            > 80 => "alarm",
            >= 60 => "ok",
            _ => "low"
        };
        Console.WriteLine(label);   // ok

        for (int i = 1; i <= 3; i++) Console.Write(i + " ");          // 1 2 3
        Console.WriteLine();

        int n = 10;
        while (n > 1) { n /= 2; Console.Write(n + " "); }             // 5 2 1
        Console.WriteLine();

        foreach (string unit in new[] { "bar", "kPa", "psi" })
        {
            if (unit == "kPa") continue;   // skip this one
            Console.Write(unit + " ");      // bar psi
        }
        Console.WriteLine();
    }
}

// === Methods ===
static class Methods
{
    // Parameters in, one value out.
    static double CircleArea(double radius) => Math.PI * radius * radius;

    // An optional parameter has a default value.
    static string Describe(string tag, string unit = "mm") => $"{tag} [{unit}]";

    // Overloads share a name but differ in parameters.
    static int Max(int a, int b) => a > b ? a : b;
    static int Max(int a, int b, int c) => Max(Max(a, b), c);

    // "out" returns a second value; the bool says whether it worked.
    static bool TryDivide(double a, double b, out double result)
    {
        result = 0;
        if (b == 0) return false;
        result = a / b;
        return true;
    }

    public static void Run()
    {
        Console.WriteLine(CircleArea(2).ToString("F3"));   // 12.566
        Console.WriteLine(Describe("DN50"));               // DN50 [mm]
        Console.WriteLine(Describe("2", unit: "inch"));    // 2 [inch]
        Console.WriteLine(Max(3, 9, 5));                   // 9
        Console.WriteLine(TryDivide(1, 0, out _));         // False
    }
}

// === Arrays, lists and dictionaries ===
static class Collections
{
    public static void Run()
    {
        int[] fixedSize = { 5, 3, 8 };          // size never changes
        Array.Sort(fixedSize);
        Console.WriteLine(string.Join(", ", fixedSize));   // 3, 5, 8

        var sizes = new List<int> { 25, 50 };    // grows as needed
        sizes.Add(80);
        sizes.Remove(25);
        Console.WriteLine($"{sizes.Count} items, first = {sizes[0]}");   // 2 items, first = 50

        var stock = new Dictionary<string, int>  // look up a value by key
        {
            ["elbow"] = 12,
            ["tee"] = 4
        };
        stock["valve"] = 7;                      // add
        stock["tee"] += 1;                       // update

        if (stock.TryGetValue("flange", out int qty)) Console.WriteLine(qty);
        else Console.WriteLine("no flanges");    // no flanges

        foreach (var (item, quantity) in stock) Console.WriteLine($"{item}: {quantity}");
    }
}

// === Value types, reference types and null ===
static class ValueVsReference
{
    public static void Run()
    {
        int a = 1;
        int b = a;      // the value is copied
        b = 99;
        Console.WriteLine(a);       // 1

        int[] x = { 1, 2, 3 };
        int[] y = x;    // the reference is copied: both names point at one array
        y[0] = 99;
        Console.WriteLine(x[0]);    // 99

        string? maybe = null;                       // "?" marks a value that may be null
        Console.WriteLine(maybe?.Length ?? -1);     // -1   ?. stops on null, ?? gives a fallback
        maybe ??= "default";                        // assign only when null
        Console.WriteLine(maybe);                   // default
    }
}

// === Exceptions and files ===
static class ErrorsAndFiles
{
    public static void Run()
    {
        string path = Path.Combine(Path.GetTempPath(), "readings.txt");
        File.WriteAllLines(path, new[] { "12.5", "oops", "7.25" });

        double total = 0;
        try
        {
            foreach (string line in File.ReadLines(path))
            {
                try
                {
                    total += double.Parse(line);
                }
                catch (FormatException)
                {
                    Console.WriteLine($"skipping bad line: {line}");
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"could not read the file: {ex.Message}");
        }
        finally
        {
            File.Delete(path);   // finally always runs, error or not
        }
        Console.WriteLine(total);   // 19.75
    }
}

// === LINQ basics ===
static class LinqBasics
{
    record Pipe(string Tag, int Diameter, double Length);

    public static void Run()
    {
        var pipes = new List<Pipe>
        {
            new("L-01", 50, 12.0), new("L-02", 80, 4.5),
            new("L-03", 50, 7.5),  new("L-04", 100, 20.0)
        };

        var large = pipes.Where(p => p.Diameter >= 80).Select(p => p.Tag);
        Console.WriteLine(string.Join(", ", large));                       // L-02, L-04

        Console.WriteLine(pipes.Sum(p => p.Length));                       // 44
        Console.WriteLine(pipes.OrderByDescending(p => p.Length).First().Tag);   // L-04
        Console.WriteLine(pipes.Any(p => p.Diameter > 150));               // False

        foreach (var group in pipes.GroupBy(p => p.Diameter))
            Console.WriteLine($"DN{group.Key}: {group.Count()} pipes, {group.Sum(p => p.Length)} m");
    }
}
