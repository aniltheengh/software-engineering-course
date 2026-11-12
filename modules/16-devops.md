# 16 — DevOps: CI/CD, containers, cloud

**Duration:** 4 weeks · **Prerequisites:** 11, 13

**Exercises:** [14 exercises for this module](../exercises/16-devops.md)

## Goals
- Take an application from a commit to a running deployment automatically
- Observe it once it is running

## Topics
1. Linux server basics
2. Continuous integration: build, test, lint on every push
3. Docker: images, containers, volumes, networks, Compose
4. Continuous delivery and deployment strategies
5. Cloud fundamentals: compute, storage, managed databases
6. Infrastructure as code (overview)
7. Observability: logs, metrics, traces
8. Kubernetes (overview only)

## Code examples

All of the C# below is one runnable project in [examples/16-devops](../examples/16-devops). Run it with `dotnet run` from that folder, then change it and run it again.

### Configuration from the environment (twelve-factor)

```csharp
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
```

### Structured logs: one JSON object per line

```csharp
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
```

### Health checks: let the platform ask "are you alive and ready?"

```csharp
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
```

### Exit codes: how a pipeline knows a step failed

```csharp
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
```

### GitHub Actions: build and test on every push

From [`snippets/01-ci.yml`](../examples/16-devops/snippets/01-ci.yml).

```yaml
# Save as .github/workflows/ci.yml
name: CI

on:
  push:
    branches: [main]
  pull_request:

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 8.0.x

      - name: Restore
        run: dotnet restore

      - name: Check formatting
        run: dotnet format --verify-no-changes

      - name: Build
        run: dotnet build --no-restore --configuration Release

      - name: Test
        run: dotnet test --no-build --configuration Release
```

### Dockerfile: multi-stage build for an ASP.NET Core app

From [`snippets/02-Dockerfile`](../examples/16-devops/snippets/02-Dockerfile).

```dockerfile
# Stage 1: build with the full SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file first so "restore" is cached until dependencies change.
COPY MyApp.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app --no-restore

# Stage 2: run on the much smaller runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "MyApp.dll"]

# Build:  docker build -t myapp .
# Run:    docker run -p 8080:8080 myapp
```

### Docker Compose: the app and its database together

From [`snippets/03-compose.yml`](../examples/16-devops/snippets/03-compose.yml).

```yaml
# Save as compose.yml and start with: docker compose up --build
services:
  app:
    build: .
    ports:
      - "8080:8080"
    environment:
      APP_ENVIRONMENT: Development
      APP_DATABASE_URL: Host=db;Database=app;Username=app;Password=${DB_PASSWORD}
    depends_on:
      db:
        condition: service_healthy   # wait until the database answers

  db:
    image: postgres:16
    environment:
      POSTGRES_DB: app
      POSTGRES_USER: app
      POSTGRES_PASSWORD: ${DB_PASSWORD}   # read from a .env file that is not committed
    volumes:
      - dbdata:/var/lib/postgresql/data   # data survives container restarts
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U app -d app"]
      interval: 5s
      timeout: 3s
      retries: 10

volumes:
  dbdata:
```

## Resources
- **Primary:** [GitHub Actions documentation](https://docs.github.com/en/actions) and [Docker — Get started](https://docs.docker.com/get-started/)
- [Containerize a .NET app](https://learn.microsoft.com/en-us/dotnet/core/docker/build-container)
- [The Twelve-Factor App](https://12factor.net/)
- [Google SRE Book (free online)](https://sre.google/sre-book/table-of-contents/)
- [roadmap.sh — DevOps](https://roadmap.sh/devops)
- [.NET observability with OpenTelemetry](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel)

## Practice
- GitHub Actions workflow that builds and tests on every pull request
- Multi-stage Dockerfile for your web app; run app + database with Docker Compose
- Deploy to a free or low-cost cloud tier
- Add health checks, structured logs and one metric

## Checkpoint project
Your Module 13 application with a **complete pipeline**: push to `main` runs tests, builds an image and deploys it, with a status badge in the README.
