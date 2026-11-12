// Run with: dotnet run
using System.Diagnostics;
using System.Text.Json;

ConfigurationDemo.Run();
StructuredLoggingDemo.Run();
HealthCheckDemo.Run();
ExitCodeDemo.Run();

// === Configuration from the environment (twelve-factor) ===
// The same build runs everywhere; only the environment differs.
static class ConfigurationDemo
{
    static string Setting(string name, string fallback) => Environment.GetEnvironmentVariable(name) ?? fallback;

    public static void Run()
    {
        string environment = Setting("APP_ENVIRONMENT", "Development");
        int port = int.Parse(Setting("APP_PORT", "8080"));
        string database = Setting("APP_DATABASE_URL", "Host=localhost;Database=dev");

        Console.WriteLine($"environment={environment} port={port}");
        Console.WriteLine($"database={database.Split(';')[0]}");   // never print full connection strings
        // Try:  APP_ENVIRONMENT=Production APP_PORT=80 dotnet run
    }
}

// === Structured logs: one JSON object per line ===
static class StructuredLoggingDemo
{
    static void Log(string level, string message, object? data = null)
    {
        var entry = new { time = DateTime.UtcNow.ToString("O"), level, message, data };
        Console.WriteLine(JsonSerializer.Serialize(entry));   // machines can filter and search this
    }

    public static void Run()
    {
        Log("info", "request finished", new { path = "/notes", status = 200, durationMs = 12 });
        Log("error", "database unreachable", new { host = "db", attempt = 3 });
    }
}

// === Health checks: let the platform ask "are you alive and ready?" ===
static class HealthCheckDemo
{
    record Check(string Name, Func<bool> Probe);

    public static void Run()
    {
        var checks = new List<Check>
        {
            new("disk space", () => new DriveInfo(Path.GetPathRoot(Path.GetTempPath())!).AvailableFreeSpace > 100_000_000),
            new("temp folder writable", () =>
            {
                string file = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".probe");
                File.WriteAllText(file, "ok");
                File.Delete(file);
                return true;
            })
        };

        bool healthy = true;
        foreach (Check check in checks)
        {
            bool ok;
            try { ok = check.Probe(); } catch { ok = false; }
            healthy &= ok;
            Console.WriteLine($"{(ok ? "healthy  " : "UNHEALTHY")} {check.Name}");
        }
        Console.WriteLine(healthy ? "200 OK" : "503 Service Unavailable");
    }
}

// === Exit codes: how a pipeline knows a step failed ===
static class ExitCodeDemo
{
    public static void Run()
    {
        var info = new ProcessStartInfo("dotnet", "--version") { RedirectStandardOutput = true };
        using Process process = Process.Start(info)!;
        string version = process.StandardOutput.ReadToEnd().Trim();
        process.WaitForExit();

        Console.WriteLine($"dotnet {version}, exit code {process.ExitCode}");
        // 0 means success. Any other value makes a CI step fail and stops the pipeline.
        // In your own tools:  return 1;  from Main, or  Environment.Exit(1);
    }
}
