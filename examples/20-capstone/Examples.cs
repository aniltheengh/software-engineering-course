// Run with: dotnet run
// A walking skeleton: the thinnest slice through every layer of a real application.
// In the capstone each region below becomes its own project
// (Domain, Application, Infrastructure, and a CLI or web front end).
return Cli.Run(args.Length > 0 ? args : new[] { "demo" });

// === Domain layer: business rules, no dependencies ===
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

// === Application layer: use cases, depending only on interfaces ===
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

// === Infrastructure layer: details that can be swapped ===
public class InMemoryDrawingRepository : IDrawingRepository
{
    private readonly Dictionary<string, Drawing> _drawings = new();
    public void Save(Drawing drawing) => _drawings[drawing.Number] = drawing;
    public Drawing? Find(string number) => _drawings.GetValueOrDefault(number);
    public IReadOnlyList<Drawing> All() => _drawings.Values.OrderBy(d => d.Number).ToList();
}
// Later: a SqliteDrawingRepository implementing the same interface. Nothing above changes.

// === Entry point: wire the layers together and report errors properly ===
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
