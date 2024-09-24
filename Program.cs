using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.DTOs;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);
var connectingString = builder.Configuration.GetConnectionString("Todos") ?? "Data Source=Todos.db";
builder.Services.AddSqlite<TodoDb>(connectingString);

//builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));

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

todoItems.MapGet("/", GetAllTodos);
todoItems.MapGet("/complete", GetCompletedTodos);
todoItems.MapGet("/{id}", GetTodoById);
todoItems.MapPost("/", CreateTodo);
todoItems.MapPut("/{id}", UpdateTodo);
todoItems.MapDelete("/{id}", DeleteTodo);

static async Task<IResult> GetAllTodos(TodoDb db)
{
  return TypedResults.Ok(await db.Todos.Select(x => new TodoDto(x)).ToArrayAsync());
}

static async Task<IResult> GetCompletedTodos(TodoDb db)
{
  return TypedResults.Ok(await db.Todos.Where(t => t.IsComlete)
                        .Select(x => new TodoDto(x)).ToListAsync());
}

static async Task<IResult> GetTodoById(int id, TodoDb db)
{
  return await db.Todos.FindAsync(id)
    is Todo todo
    ? TypedResults.Ok(new TodoDto(todo))
    : TypedResults.NotFound();
}

static async Task<IResult> CreateTodo(TodoDto todoDto, TodoDb db)
{
  var newTodo = new Todo
  {
    IsComlete = todoDto.IsComlete,
    Name = todoDto.Name
  };
  db.Todos.Add(newTodo);
  await db.SaveChangesAsync();
  todoDto = new TodoDto(newTodo);
  return TypedResults.Created($"/todoitems/{newTodo.Id}", todoDto);
}

static async Task<IResult> UpdateTodo(int id, TodoDto inputTodoDto, TodoDb db)
{
  var todo = await db.Todos.FindAsync(id);
  if (todo is null) return TypedResults.NotFound();
  todo.Name = inputTodoDto.Name;
  todo.IsComlete = inputTodoDto.IsComlete;
  await db.SaveChangesAsync();
  return TypedResults.NoContent();
}

static async Task<IResult> DeleteTodo(int id, TodoDb db)
{
  if (await db.Todos.FindAsync(id) is Todo todo)
    {
      db.Todos.Remove(todo);
      await db.SaveChangesAsync();
      return TypedResults.NoContent();
    }
    return TypedResults.NotFound();
}

app.Run();
