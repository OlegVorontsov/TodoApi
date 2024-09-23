using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
  config.DocumentName = "TodoApi";
  config.Title = "TodoApi v1.0";
  config.Version = "1.0";
});
var app = builder.Build();

if(app.Environment.IsDevelopment())
{
  app.UseOpenApi();
  app.UseSwaggerUi(config =>
  {
    config.DocumentTitle ="TodoApi";
    config.Path = "/swagger";
    config.DocumentPath = "/swagger/{documentName}/swagger.json";
    config.DocExpansion = "list";
  });
}

var todoItems = app.MapGroup("/todoitems");

app.MapGet("/", async (TodoDb db) =>
                        await db.Todos.ToListAsync());

app.MapGet("/complete", async (TodoDb db) =>
                        await db.Todos.Where(t => t.IsComlete).ToListAsync());

app.MapGet("/{id}", async (int id, TodoDb db) =>
                        await db.Todos.FindAsync(id)
                        is Todo todo
                        ? Results.Ok(todo)
                        : Results.NotFound());

app.MapPost("/", async (Todo todo, TodoDb db) =>
                        {
                          db.Todos.Add(todo);
                          await db.SaveChangesAsync();
                          return Results.Created($"/todoitems/{todo.Id}", todo);
                        });

app.MapPut("/{id}", async (int id, Todo inputTodo, TodoDb db) =>
                        {
                          var todo = await db.Todos.FindAsync(id);
                          if (todo is null) return Results.NotFound();

                          todo.Name = inputTodo.Name;
                          todo.IsComlete = inputTodo.IsComlete;

                          await db.SaveChangesAsync();
                          return Results.NoContent(); 
                        });

app.MapDelete("/{id}", async (int id, TodoDb db) =>
                        {
                          if (await db.Todos.FindAsync(id) is Todo todo)
                          {
                            db.Todos.Remove(todo);
                            await db.SaveChangesAsync();
                            return Results.NoContent();
                          }
                          return Results.NotFound();
                        });

app.Run();
