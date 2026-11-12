# Code examples

One runnable C# project per module. The same code is shown inside each module page, section by section.

## Running an example

```bash
cd examples/01-programming-fundamentals
dotnet run
```

Each `Examples.cs` runs every demo in the file from top to bottom. Comments such as `// 18` show the output to expect. Change a value, run it again, and see what happens: that is the fastest way to learn from them.

- The projects target .NET 8. On a newer SDK, change `<TargetFramework>` in `Examples.csproj` to match (for example `net10.0`).
- Module 13 is a web application: `dotnet run`, then open http://localhost:5080.
- Module 20 ends with exit code 1 on purpose, to show how a program reports failure.

## The snippets folders

Files under `snippets/` are for reading and copying, and are not compiled with the project: shell and Git commands, SQL, a workflow file, a Dockerfile, a Compose file, a `.proto` contract, an HTML page, and three C# files that need NuGet packages (xUnit, Microsoft.Data.Sqlite, EF Core).
