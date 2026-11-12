// === Application setup ===
// Run with: dotnet run     then open http://localhost:5080
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5080");
builder.Services.AddSingleton<NoteStore>();   // one shared instance, handed to endpoints that ask for it
var app = builder.Build();

// === Middleware: code that runs around every request ===
app.Use(async (context, next) =>
{
    var timer = Stopwatch.StartNew();
    await next();   // run the rest of the pipeline (the endpoint)
    Console.WriteLine($"{context.Request.Method} {context.Request.Path} -> {context.Response.StatusCode} in {timer.ElapsedMilliseconds} ms");
});

// === Routes, route parameters and query strings ===
app.MapGet("/", () => "Hello from ASP.NET Core");

// GET /hello/Taman           -> the {name} segment is bound to the parameter
app.MapGet("/hello/{name}", (string name) => $"Hello, {name}!");

// GET /add?a=2&b=3           -> query string values; the object is returned as JSON
app.MapGet("/add", (int a, int b) => new { a, b, sum = a + b });

// === A small REST resource with correct status codes ===
app.MapGet("/notes", (NoteStore store) => store.All());

app.MapGet("/notes/{id:int}", (int id, NoteStore store) =>
    store.Find(id) is { } note ? Results.Ok(note) : Results.NotFound());

// POST /notes with body {"text":"..."}  -> the JSON body is bound to NoteInput
app.MapPost("/notes", (NoteInput input, NoteStore store) =>
{
    if (string.IsNullOrWhiteSpace(input.Text))
        return Results.BadRequest(new { error = "Text is required" });   // 400

    Note note = store.Add(input.Text);
    return Results.Created($"/notes/{note.Id}", note);                    // 201 + Location header
});

app.MapDelete("/notes/{id:int}", (int id, NoteStore store) =>
    store.Remove(id) ? Results.NoContent() : Results.NotFound());         // 204 or 404

app.Run();

// === Models and a service registered for dependency injection ===
record Note(int Id, string Text, DateTime CreatedAt);
record NoteInput(string Text);

class NoteStore
{
    private readonly List<Note> _notes = new();
    private readonly object _gate = new();   // requests run in parallel, so protect shared state
    private int _nextId = 1;

    public List<Note> All() { lock (_gate) return _notes.ToList(); }
    public Note? Find(int id) { lock (_gate) return _notes.FirstOrDefault(n => n.Id == id); }
    public bool Remove(int id) { lock (_gate) return _notes.RemoveAll(n => n.Id == id) > 0; }

    public Note Add(string text)
    {
        lock (_gate)
        {
            var note = new Note(_nextId++, text, DateTime.UtcNow);
            _notes.Add(note);
            return note;
        }
    }
}
