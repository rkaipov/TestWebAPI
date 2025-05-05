namespace TestWebAPI.DataModels.Models;

public class OrderDto
{
  public string Id { get; set; } = Guid.NewGuid().ToString();
  public OrderStatus Status { get; set; } = OrderStatus.New;
  public required string Address { get; set; }
  public decimal Total { get; set; }
}
