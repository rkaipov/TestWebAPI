using Microsoft.EntityFrameworkCore;
using TestWebAPI.Data;
using TestWebAPI.DataModels;

namespace TestWebAPI.Repositories;
public class ItemRepository : IItemRepository
{
  private readonly TestWebAPIContext _db;

  public ItemRepository(TestWebAPIContext dbContext)
  {
    _db = dbContext;
    _db.Database.EnsureCreated();
  }

  public async Task<Item?> CreateAsync(Item entity)
  {
    var entry = await _db.Item.AddAsync(entity);
    await _db.SaveChangesAsync();
    return entry.Entity;
  }

  public async Task<bool?> DeleteAsync(string id)
  {
    var entry = await _db.Item.FindAsync(id);
    if (entry is null)
    {
      return false;
    }

    _db.Item.Remove(entry);
    await _db.SaveChangesAsync();
    return true;
  }

  public async Task<Item[]> GetAllAsync(CancellationToken ct)
  {
    return await _db.Item.AsNoTracking().ToArrayAsync(ct);
  }

  public async Task<Item?> GetByIdAsync(string id, CancellationToken ct)
  {
    return await _db.Item.AsNoTracking().SingleOrDefaultAsync(x => x.Id.Equals(id), ct);
  }

  public async Task<Item?> UpdateAsync(Item entity)
  {
    var tracked = await _db.Item.FindAsync(entity.Id);

    if (tracked is not null)
    {
      tracked.Name = entity.Name;
      tracked.Price = entity.Price;
    }

    await _db.SaveChangesAsync();
    return tracked;
  }
}
