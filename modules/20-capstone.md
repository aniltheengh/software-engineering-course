# 20 — Capstone project

**Duration:** 6 weeks · **Prerequisites:** Phases 1–3

**Exercises:** [8 exercises for this module](../exercises/20-capstone.md)

## Goal
Design, build, test, deploy and document one substantial piece of software on your own. This is the project you show to other people.

## Requirements
- A written proposal: problem, users, scope, what is deliberately left out
- A design document with an architecture diagram and data model
- At least 3,000 lines of your own C#
- Automated tests and a CI pipeline
- Persistent storage
- A deployed or installable release
- A README that lets a stranger build and run it
- A short retrospective: what went well, what you would change

## Project ideas
- **CAD automation tool:** a plugin or companion service that automates drawing or model generation, with a tested core library separate from the CAD host
- **Engineering calculation service:** web API and frontend for sizing calculations, with saved projects and PDF reports
- **Document pipeline:** ingest files, extract data, store, search and report, processed by background workers
- **Developer tool:** a CLI or Roslyn analyzer published as a NuGet package
- **Distributed system:** extend the Module 17 key-value store with full Raft and a client library

## Suggested schedule
| Week | Work |
|------|------|
| 1 | Proposal, design document, repository and CI set up |
| 2–4 | Core features, tests written alongside |
| 5 | Hardening: error handling, security review, performance |
| 6 | Deployment, documentation, retrospective |

## Code examples

All of the C# below is one runnable project in [examples/20-capstone](../examples/20-capstone). Run it with `dotnet run` from that folder, then change it and run it again.

### Domain layer: business rules, no dependencies

```csharp
public enum DrawingStatus { Draft, InReview, Approved }

public class Drawing
{
    public string Number { get; }
    public string Title { get; }
    public int Revision { get; private set; }
    public DrawingStatus Status { get; private set; } = DrawingStatus.Draft;

    public Drawing(string number, string title)
    {
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Number is required", nameof(number));
        Number = number;
        Title = title;
    }

    public void SubmitForReview()
    {
        if (Status != DrawingStatus.Draft) throw new InvalidOperationException("Only a draft can be submitted");
        Status = DrawingStatus.InReview;
    }

    public void Approve()
    {
        if (Status != DrawingStatus.InReview) throw new InvalidOperationException("Only a drawing in review can be approved");
        Status = DrawingStatus.Approved;
    }

    public void Revise()
    {
        Revision++;
        Status = DrawingStatus.Draft;   // a new revision starts the cycle again
    }
}
```

### Application layer: use cases, depending only on interfaces

```csharp
public interface IDrawingRepository
{
    void Save(Drawing drawing);
    Drawing? Find(string number);
    IReadOnlyList<Drawing> All();
}

public class DrawingService
{
    private readonly IDrawingRepository _repository;
    public DrawingService(IDrawingRepository repository) => _repository = repository;

    public Drawing Register(string number, string title)
    {
        if (_repository.Find(number) != null) throw new InvalidOperationException($"Drawing {number} already exists");
        var drawing = new Drawing(number, title);
        _repository.Save(drawing);
        return drawing;
    }

    public void Approve(string number)
    {
        Drawing drawing = _repository.Find(number) ?? throw new KeyNotFoundException($"Drawing {number} not found");
        drawing.SubmitForReview();
        drawing.Approve();
        _repository.Save(drawing);
    }

    public IReadOnlyList<Drawing> List() => _repository.All();
}
```

### Infrastructure layer: details that can be swapped

```csharp
public class InMemoryDrawingRepository : IDrawingRepository
{
    private readonly Dictionary<string, Drawing> _drawings = new();
    public void Save(Drawing drawing) => _drawings[drawing.Number] = drawing;
    public Drawing? Find(string number) => _drawings.GetValueOrDefault(number);
    public IReadOnlyList<Drawing> All() => _drawings.Values.OrderBy(d => d.Number).ToList();
}
// Later: a SqliteDrawingRepository implementing the same interface. Nothing above changes.
```

### Entry point: wire the layers together and report errors properly

```csharp
static class Cli
{
    public static int Run(string[] args)
    {
        var service = new DrawingService(new InMemoryDrawingRepository());   // composition root
        try
        {
            service.Register("P-1001", "Raw water pump skid");
            service.Register("P-1002", "Filter vessel piping");
            service.Approve("P-1001");

            foreach (Drawing d in service.List())
                Console.WriteLine($"{d.Number} rev {d.Revision}  {d.Status,-9} {d.Title}");

            service.Register("P-1001", "Duplicate");   // a business rule is violated
            return 0;
        }
        catch (InvalidOperationException ex)
        {
            Console.Error.WriteLine($"error: {ex.Message}");
            return 1;   // non-zero exit code so scripts and CI notice the failure
        }
    }
}
```

## Resources
- [Make a README](https://www.makeareadme.com/)
- [Keep a Changelog](https://keepachangelog.com/)
- [Semantic Versioning](https://semver.org/)
- [Choose a License](https://choosealicense.com/)
