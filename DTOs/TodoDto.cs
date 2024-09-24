using TodoApi.Models;

namespace TodoApi.DTOs
{
  public class TodoDto
  {
    public int Id { get; set;}
    public string? Name { get; set;}
    public bool IsComlete { get; set;}
    public TodoDto() { }
    public TodoDto(Todo todoItem) =>
      (Id, Name, IsComlete) = (todoItem.Id, todoItem.Name, todoItem.IsComlete);
  }
}