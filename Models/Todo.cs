namespace TodoApi.Models
{
  public class Todo
  {
    public int Id { get; set;}
    public string? Name { get; set;}
    public bool IsComlete { get; set;}
    public string? SpecialInfo { get; set;}
  }
}