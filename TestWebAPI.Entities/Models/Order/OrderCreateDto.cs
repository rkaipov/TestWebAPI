using System.ComponentModel.DataAnnotations;

namespace TestWebAPI.DataModels.Models;

public class OrderCreateDto
{
  public OrderStatus Status { get; set; } = OrderStatus.New;
  [Required]
  public required string Address { get; set; }
  public decimal Total { get; set; }
}

