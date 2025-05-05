using Microsoft.EntityFrameworkCore;
using TestWebAPI.Data;
using TestWebAPI.DataModels;

namespace TestWebAPI.Repositories;

public class OrderRepository : IOrderRepository
{
  private readonly TestWebAPIContext _db;

  public OrderRepository(TestWebAPIContext dbContext)
  {
    _db = dbContext;
    _db.Database.EnsureCreated();
  }
  public async Task<Order?> CreateAsync(Order entity)
  {
    var entry = await _db.Order.AddAsync(entity);
    await _db.SaveChangesAsync();
    return entry.Entity;
  }

  public async Task<bool?> DeleteAsync(string id)
  {
    var entry = await _db.Order.FindAsync(id);
    if (entry is null)
    {
      return false;
    }

    _db.Order.Remove(entry);
    await _db.SaveChangesAsync();
    return true;
  }

  public async Task<Order[]> GetAllAsync(CancellationToken ct)
  {
    return await _db.Order.AsNoTracking().ToArrayAsync(ct);
  }

  public async Task<Order?> GetByIdAsync(string id, CancellationToken ct)
  {
    return await _db.Order.AsNoTracking().SingleOrDefaultAsync(x => x.Id.Equals(id), ct);
  }

  public async Task<Order?> UpdateAsync(Order entity)
  {
    var tracked = await _db.Order.FindAsync(entity.Id);

    if (tracked is null)
    {
      return null;
    }

    tracked.Address = entity.Address;
    tracked.Status = entity.Status;
    tracked.Total = entity.Total;

    await _db.SaveChangesAsync();

    return tracked;
  }
}
