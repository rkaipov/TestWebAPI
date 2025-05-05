using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TestWebAPI.DataModels;
using TestWebAPI.DataModels.Models;
using TestWebAPI.Repositories;

namespace TestWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ItemsController : ControllerBase
{
  private readonly IItemRepository _repository;
  private readonly IMapper _mapper;
  private readonly ILogger<ItemsController> _logger;

  public ItemsController(IItemRepository repo, IMapper mapper, ILogger<ItemsController> logger)
  {
    _repository = repo;
    _mapper = mapper;
    _logger = logger;
  }

  /// <summary>
  /// Lists all items
  /// </summary>
  /// <param name="Name">Optional Name filter</param>
  /// <returns>List of Items filtered by (optional) Name</returns>
  [HttpGet()]
  public async Task<ActionResult<IEnumerable<ItemDto>>> GetItemList([FromQuery] string? Name, CancellationToken ct)
  {
    var items = await _repository.GetAllAsync(ct);
    if (Name is null)
    {
      return Ok(_mapper.Map<List<ItemDto>>(items));
    }

    return Ok(_mapper.Map<List<ItemDto>>(items.Where(x => x.Name.ToLower().StartsWith(Name.ToLower())).ToArray()));
  }

  /// <summary>
  /// Retrieves single Item by Id
  /// </summary>
  /// <param name="id">The Id of Item</param>
  /// <returns>Single Item or NotFound.</returns>
  [HttpGet("{id}")]
  [ProducesResponseType<ItemDto>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ItemDto>> GetItem(string id, CancellationToken ct)
  {
    var item = await _repository.GetByIdAsync(id.ToLower(), ct);
    if (item == null)
    {
      _logger.LogError("No such Item: Id = {id}", id);
      return NotFound(new { message = "No such Item" });
    }

    return Ok(_mapper.Map<ItemDto>(item));
  }

  /// <summary>
  /// Updates single Item by Id
  /// </summary>
  /// <param name="id">The Id of Item</param>
  /// <param name="item">ItemUpdateDto with properties that should be updated</param>
  /// <returns>Single updated Item or NotFound.</returns>
  [HttpPut("{id}")]
  [ProducesResponseType<ItemDto>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ItemDto>> UpdateItem(string id, ItemUpdateDto item)
  {
    var oldItem = await _repository.GetByIdAsync(id.ToLower(), default);
    if (oldItem == null)
    {
      _logger.LogError("No such Item: Id = {id}", id);
      return NotFound(new { message = "No such Item" });
    }

    var newItem = new Item
    {
      Id = oldItem.Id,
      Name = item.Name ?? oldItem.Name,
      Price = item.Price ?? oldItem.Price
    };

    var updated = await _repository.UpdateAsync(newItem);

    return Ok(updated);
  }

  /// <summary>
  /// Creates new Item
  /// </summary>
  /// <param name="item">ItemUpdateDto with properties that should be updated</param>
  /// <returns>Created Item or BadRequest</returns>
  [HttpPost]
  [ProducesResponseType<ItemDto>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<ItemDto>> CreateItem(ItemCreateDto item)
  {
    if (!ModelState.IsValid || item.Price < 0)
    {
      _logger.LogError("Item data is invalid");
      return BadRequest(new { message = "Item data is invalid" });
    }
    var created = await _repository.CreateAsync(new Item { Name = item.Name, Price = item.Price });
    if (created == null)
    {
      _logger.LogError("Error creating new Item");
      return BadRequest(new { message = "Error creating Item" });
    }

    return CreatedAtAction(nameof(CreateItem), new { id = created?.Id }, _mapper.Map<ItemDto>(created));
  }

  /// <summary>
  /// Deletes single Item by Id
  /// </summary>
  /// <param name="id">The Id of Item to delete</param>
  /// <returns>NoContent or NotFound.</returns>
  [HttpDelete("{id}")]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> DeleteItem(string id)
  {
    if (!(await ItemExists(id.ToLower())))
    {
      _logger.LogError("No such Item: Id = {id}", id);
      return NotFound(new { message = "No such Item" });
    }
    await _repository.DeleteAsync(id.ToLower());

    return NoContent();
  }

  private async Task<bool> ItemExists(string id)
  {
    var item = await _repository.GetByIdAsync(id.ToLower(), default);
    if (item == null)
    {
      return false;
    }
    return true;
  }
}
