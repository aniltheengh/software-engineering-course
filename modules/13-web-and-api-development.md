# 13 — Web and API development

**Duration:** 5 weeks · **Prerequisites:** 09, 10, 12

**Exercises:** [14 exercises for this module](../exercises/13-web-and-api-development.md)

## Goals
- Build, document and test a REST API with ASP.NET Core
- Understand enough frontend to put a usable interface on it

## Topics
1. HTML, CSS and JavaScript essentials
2. ASP.NET Core: hosting, middleware pipeline, routing
3. Minimal APIs and controllers
4. Model binding, validation, error handling
5. EF Core in a web app
6. Authentication and authorisation: cookies, JWT, OAuth 2.0 / OpenID Connect
7. OpenAPI documentation
8. Integration testing with `WebApplicationFactory`
9. A frontend option: Blazor or Razor Pages

## Code examples

All of the C# below is one runnable project in [examples/13-web-and-api-development](../examples/13-web-and-api-development). Start it with `dotnet run` and call it from a browser or with the `curl` commands shown.

### Application setup

```csharp
// Run with: dotnet run     then open http://localhost:5080
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5080");
builder.Services.AddSingleton<NoteStore>();   // one shared instance, handed to endpoints that ask for it
var app = builder.Build();
```

### Middleware: code that runs around every request

```csharp
app.Use(async (context, next) =>
{
    var timer = Stopwatch.StartNew();
    await next();   // run the rest of the pipeline (the endpoint)
    Console.WriteLine($"{context.Request.Method} {context.Request.Path} -> {context.Response.StatusCode} in {timer.ElapsedMilliseconds} ms");
});
```

### Routes, route parameters and query strings

```csharp
app.MapGet("/", () => "Hello from ASP.NET Core");

// GET /hello/Taman           -> the {name} segment is bound to the parameter
app.MapGet("/hello/{name}", (string name) => $"Hello, {name}!");

// GET /add?a=2&b=3           -> query string values; the object is returned as JSON
app.MapGet("/add", (int a, int b) => new { a, b, sum = a + b });
```

### A small REST resource with correct status codes

```csharp
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
```

### Models and a service registered for dependency injection

```csharp
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
```

### Calling the API with curl

From [`snippets/01-requests.sh`](../examples/13-web-and-api-development/snippets/01-requests.sh).

```bash
# Start the example first with: dotnet run
curl http://localhost:5080/
curl http://localhost:5080/hello/Taman
curl "http://localhost:5080/add?a=2&b=3"                  # {"a":2,"b":3,"sum":5}

# Create (-i also prints the status line and headers)
curl -i -X POST http://localhost:5080/notes \
     -H "Content-Type: application/json" \
     -d '{"text":"Check flange rating"}'                  # 201 Created, Location: /notes/1

curl http://localhost:5080/notes                          # list
curl -i http://localhost:5080/notes/99                    # 404 Not Found
curl -i -X POST http://localhost:5080/notes \
     -H "Content-Type: application/json" -d '{"text":""}' # 400 Bad Request
curl -i -X DELETE http://localhost:5080/notes/1           # 204 No Content
```

### A page that calls the API with fetch

From [`snippets/02-page.html`](../examples/13-web-and-api-development/snippets/02-page.html).

```html
<!-- The API must allow this page's origin (CORS) or serve the page itself. -->
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Notes</title>
  <style>
    body { font-family: system-ui, sans-serif; max-width: 40rem; margin: 2rem auto; padding: 0 1rem; }
    form { display: flex; gap: 0.5rem; }
    input { flex: 1; padding: 0.5rem; }
    li { padding: 0.25rem 0; }
  </style>
</head>
<body>
  <h1>Notes</h1>
  <form id="form">
    <input id="text" placeholder="New note" required>
    <button>Add</button>
  </form>
  <ul id="list"></ul>

  <script>
    const api = "http://localhost:5080/notes";
    const list = document.getElementById("list");

    async function load() {
      const response = await fetch(api);
      const notes = await response.json();
      list.replaceChildren(...notes.map(note => {
        const item = document.createElement("li");
        item.textContent = note.text;        // textContent, not innerHTML: no script injection
        return item;
      }));
    }

    document.getElementById("form").addEventListener("submit", async event => {
      event.preventDefault();                // stop the browser reloading the page
      const input = document.getElementById("text");
      const response = await fetch(api, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ text: input.value })
      });
      if (response.ok) { input.value = ""; await load(); }
    });

    load();
  </script>
</body>
</html>
```

## Resources
- **Primary:** [ASP.NET Core documentation and tutorials](https://learn.microsoft.com/en-us/aspnet/core/)
- [Tutorial: Create a minimal API with ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api)
- [MDN — Learn web development](https://developer.mozilla.org/en-US/docs/Learn)
- [Blazor documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [roadmap.sh — ASP.NET Core](https://roadmap.sh/aspnet-core)

## Practice
- Build a static page by hand with HTML, CSS and a little JavaScript
- CRUD minimal API with validation and proper status codes
- Add JWT authentication and role-based authorisation
- Write integration tests that run against an in-memory or container database

## Checkpoint project
A **full-stack web application** (API + database + Blazor or Razor frontend) with login, tests and OpenAPI docs.
