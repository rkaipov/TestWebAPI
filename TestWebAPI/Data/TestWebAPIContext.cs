using Microsoft.EntityFrameworkCore;
using TestWebAPI.DataModels;

namespace TestWebAPI.Data;

public class TestWebAPIContext : DbContext
{
  public TestWebAPIContext(DbContextOptions<TestWebAPIContext> options)
      : base(options)
  {
  }

  public DbSet<Item> Item { get; set; } = default!;
  public DbSet<Order> Order { get; set; } = default!;
}
