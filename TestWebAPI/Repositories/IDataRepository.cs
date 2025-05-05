namespace TestWebAPI.Repositories;

public interface IDataRepository<TEntity>
{
  public Task<TEntity?> GetByIdAsync(string id, CancellationToken ct);
  public Task<TEntity[]> GetAllAsync(CancellationToken ct);
  public Task<TEntity?> CreateAsync(TEntity entity);
  public Task<TEntity?> UpdateAsync(TEntity entity);
  public Task<bool?> DeleteAsync(string id);

}
