using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDbContext>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/", async (TodoDbContext db) => {
    var todos = await db.Todos.ToListAsync();
    return Results.Ok(todos);
});
app.MapGet("/{id}", async (int id, TodoDbContext db) => {
    var todo = await db.Todos.FindAsync(id);
    return Results.Ok(todo);
});
app.MapPost("/", async (TodoDbContext db, Todo todo) => {
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/{todo.Id}", todo);
});
app.MapPut("/{id}", async (int id, TodoDbContext db, Todo newTodo) => {
    var todo = await db.Todos.FindAsync(id);
    if(todo is null) return Results.NotFound();
    todo.Name = newTodo.Name;
    todo.IsComplete = newTodo.IsComplete;
    await db.SaveChangesAsync();
    return Results.Ok(todo);
});
app.MapDelete("/{id}", async (int id, TodoDbContext db) => {
    if(await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.Ok(todo);
    }
    else
    {
        return Results.NotFound();
    }
});
app.Run();

class Todo
{
    public int Id{get; set;}
    public string? Name {get; set;}
    public bool IsComplete {get; set;}
}

class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) {}
    public DbSet<Todo> Todos => Set<Todo>();
}