// Run with: dotnet run
using System.Diagnostics;

DebuggingAids.Run();
ReadingStackTraces.Run();

// === Debugging aids in code ===
// Set a breakpoint on the line inside the loop, then use Step Over (F10),
// Step Into (F11) and the Watch window to follow "sum" and "i".
static class DebuggingAids
{
    static double Average(int[] values)
    {
        // An assertion documents an assumption and stops a Debug build when it is false.
        Debug.Assert(values.Length > 0, "values must not be empty");

        int sum = 0;
        for (int i = 0; i < values.Length; i++)
        {
            sum += values[i];   // <- breakpoint here; add a condition such as  i == 2
        }
        return (double)sum / values.Length;
    }

    public static void Run()
    {
        var timer = Stopwatch.StartNew();
        double avg = Average(new[] { 4, 8, 15, 16, 23, 42 });
        timer.Stop();

        Console.WriteLine($"average = {avg}");                              // 18
        Console.WriteLine($"took {timer.Elapsed.TotalMilliseconds:F3} ms");
        Debug.WriteLine("This goes to the debugger's Output window, not the console.");
    }
}

// === Reading a stack trace ===
// Read it from the top: the first line is where the exception was thrown,
// each following line is the caller of the one above.
static class ReadingStackTraces
{
    static int ParsePort(string text) => int.Parse(text);
    static int LoadConfig() => ParsePort("80a");

    public static void Run()
    {
        try
        {
            LoadConfig();
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}
