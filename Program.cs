var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var notes = new List<Note>();
var nextId = 1;

app.MapGet("/api/notes", () => notes);

app.MapGet("/api/notes/{id}", (int id) =>
{
    var note = notes.FirstOrDefault(n => n.Id == id);
    return note is not null ? Results.Ok(note) : Results.NotFound();
});

app.MapPost("/api/notes", (CreateNoteRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest(new { error = "Title is required" });
    }
    
    var note = new Note
    {
        Id = nextId++,
        Title = request.Title,
        Text = request.Text ?? "",
        CreatedAt = DateTime.UtcNow
    };
    notes.Add(note);
    return Results.Created($"/api/notes/{note.Id}", note);
});

app.MapDelete("/api/notes/{id}", (int id) =>
{
    var note = notes.FirstOrDefault(n => n.Id == id);
    if (note is null) return Results.NotFound();
    notes.Remove(note);
    return Results.NoContent();
});

app.MapGet("/health", () => new { status = "ok", time = DateTime.UtcNow });
app.MapGet("/version", () => new { app = "IsLabApp", version = "1.0.0" });
app.MapGet("/db/ping", () => new { db = "Not connected", status = "error" });

app.Run();

record Note
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Text { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

record CreateNoteRequest(string Title, string Text);