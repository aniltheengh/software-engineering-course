# 01 — Programming fundamentals in C#

**Duration:** 6 weeks · **Prerequisites:** none

**Exercises:** [12 exercises for this module](../exercises/01-programming-fundamentals.md)

## Goals
- Write, run and debug console programs in C# without copying from a tutorial
- Break a problem into functions and choose suitable built-in types and collections

## Topics
1. The .NET toolchain: `dotnet new`, `dotnet run`, project files
2. Types, variables, operators, strings, type conversion
3. Control flow: `if`, `switch`, loops
4. Methods: parameters, return values, overloads, scope
5. Arrays, `List<T>`, `Dictionary<TKey,TValue>`
6. Value types vs reference types, `null` and nullable reference types
7. Exceptions and basic file I/O
8. LINQ basics: `Where`, `Select`, `OrderBy`, `GroupBy`

## Code examples

All of the C# below is one runnable project in [examples/01-programming-fundamentals](../examples/01-programming-fundamentals). Run it with `dotnet run` from that folder, then change it and run it again.

### Variables, types and conversion

```csharp
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
```

### Control flow

```csharp
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
```

### Methods

```csharp
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
```

### Arrays, lists and dictionaries

```csharp
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
```

### Value types, reference types and null

```csharp
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
```

### Exceptions and files

```csharp
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
```

### LINQ basics

```csharp
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
```

## Resources
- **Primary:** [Microsoft Learn — Get started with C#](https://learn.microsoft.com/en-us/training/paths/get-started-c-sharp-part-1/) (continue through the following parts)
- [C# fundamentals documentation](https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/)
- [Exercism C# track](https://exercism.org/tracks/csharp) — graded practice exercises
- [CS50x](https://cs50.harvard.edu/x/) — weeks 0–5 for the underlying ideas (taught in C)

## Practice
- Complete 30 Exercism C# exercises
- Unit converter (length, pressure, flow rate) with input validation
- Read a CSV file, compute min/max/average per column, write a summary file
- Word-frequency counter using `Dictionary` and then again using LINQ

## Checkpoint project
A command-line **expense or inventory tracker**: add, list, filter and delete records, saved to a JSON file. Must handle bad input without crashing.
