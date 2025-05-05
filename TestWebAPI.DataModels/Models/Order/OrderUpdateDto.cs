namespace TestWebAPI.DataModels.Models;

public class OrderUpdateDto
{
  public OrderStatus? Status { get; set; }
  public string? Address { get; set; }
  public decimal? Total { get; set; }
}

