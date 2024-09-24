using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data
{
  public class TodoDb : DbContext
  {
    public TodoDb(DbContextOptions options)
    : base(options) {}
    public DbSet<Todo> Todos => Set<Todo>();
  }
}
