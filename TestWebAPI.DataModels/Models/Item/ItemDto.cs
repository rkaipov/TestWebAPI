namespace TestWebAPI.DataModels.Models;

public class ItemDto
{
  public string Id { get; set; } = Guid.NewGuid().ToString();
  public required string Name { get; set; }
  public decimal Price { get; set; }
}
