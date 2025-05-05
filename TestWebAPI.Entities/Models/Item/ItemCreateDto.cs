using System.ComponentModel.DataAnnotations;

namespace TestWebAPI.DataModels.Models;

public class ItemCreateDto
{
  [Required]
  public required string Name { get; set; }
  public decimal Price { get; set; }
}