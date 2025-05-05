using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TestWebAPI.DataModels;
using TestWebAPI.DataModels.Models;
using TestWebAPI.Repositories;

namespace TestWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
  private readonly IOrderRepository _repository;
  private readonly IMapper _mapper;
  private readonly ILogger<OrdersController> _logger;

  public OrdersController(IOrderRepository repo, IMapper mapper, ILogger<OrdersController> logger)
  {
    _repository = repo;
    _mapper = mapper;
    _logger = logger;
  }

  /// <summary>
  /// Lists all Orders
  /// </summary>
  /// <param name="status">Optional Name filter</param>
  /// <returns>List of Orders filtered by (optional) status</returns>
  [HttpGet()]
  public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrderList([FromQuery][EnumDataType(typeof(OrderStatus))] OrderStatus? status, CancellationToken ct)
  {
    var orders = await _repository.GetAllAsync(ct);
    if (status == null)
    {
      return Ok(_mapper.Map<List<OrderDto>>(orders));
    }
    return Ok(_mapper.Map<List<OrderDto>>(orders.Where(x => (x.Status & status) != 0).ToList()));
  }

  /// <summary>
  /// Retrieves single Order by Id
  /// </summary>
  /// <param name="id">The Id of Order</param>
  /// <returns>Single Order or NotFound.</returns>
  [HttpGet("{id}")]
  [ProducesResponseType<OrderDto>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<OrderDto>> GetOrder(string id, CancellationToken ct)
  {
    var order = await _repository.GetByIdAsync(id.ToLower(), ct);
    if (order == null)
    {
      _logger.LogError("No such Order: Id = {id}", id);
      return NotFound(new { message = "No such Order" });
    }

    return Ok(_mapper.Map<OrderDto>(order));
  }

  /// <summary>
  /// Updates single Order by Id
  /// </summary>
  /// <param name="id">The Id of Order</param>
  /// <param name="order">OrderUpdateDto with properties that should be updated</param>
  /// <returns>Single updated Order or NotFound.</returns>
  [HttpPut("{id}")]
  [ProducesResponseType<OrderDto>(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<OrderDto>> UpdateOrder(string id, OrderUpdateDto order)
  {
    var oldOrder = await _repository.GetByIdAsync(id.ToLower(), default);
    if (oldOrder == null)
    {
      _logger.LogError("No such Order: Id = {id}", id);
      return NotFound(new { message = "No such Order" });
    }

    var newOrder = new Order
    {
      Id = oldOrder.Id,
      Address = order.Address ?? oldOrder.Address,
      Status = order.Status ?? oldOrder.Status,
      Total = order.Total ?? oldOrder.Total,
    };

    var updated = await _repository.UpdateAsync(newOrder);

    return Ok(_mapper.Map<OrderDto>(updated));
  }

  /// <summary>
  /// Creates new Order
  /// </summary>
  /// <param name="order">OrderUpdateDto with properties that should be updated</param>
  /// <returns>Created Item or BadRequest</returns>
  [HttpPost]
  [ProducesResponseType<OrderDto>(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<OrderDto>> CreateOrder(OrderCreateDto order)
  {
    if (!ModelState.IsValid || order.Total < 0)
    {
      _logger.LogError("Order data is invalid");
      return BadRequest(new { message = "Order data is invalid" });
    }
    var created = await _repository.CreateAsync(new Order { Address = order.Address, Status = order.Status, Total = order.Total });
    if (created == null)
    {
      _logger.LogError("Error creating new Order");
      return BadRequest(new { message = "Error creating Order" });
    }

    return CreatedAtAction(nameof(CreateOrder), new { id = created?.Id }, _mapper.Map<OrderDto>(created));
  }

  /// <summary>
  /// Deletes single Order by Id
  /// </summary>
  /// <param name="id">The Id of Order to delete</param>
  /// <returns>NoContent or NotFound.</returns>
  [HttpDelete("{id}")]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> DeleteOrder(string id)
  {
    if (!(await OrderExists(id.ToLower())))
    {
      _logger.LogError("No such Order: Id = {id}", id);
      return NotFound(new { message = "No such Order" });
    }
    await _repository.DeleteAsync(id.ToLower());

    return NoContent();
  }

  private async Task<bool> OrderExists(string id)
  {
    var order = await _repository.GetByIdAsync(id.ToLower(), default);
    if (order == null)
    {
      return false;
    }
    return true;
  }
}
