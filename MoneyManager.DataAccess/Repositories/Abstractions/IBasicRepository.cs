using MoneyManager.DataAccess.Entities;

namespace MoneyManager.DataAccess.Repositories.Abstractions;

public interface IBasicRepository<T> where T : EntityBase
{
    Task<List<T>> GetAllEntitiesAsync(CancellationToken cancellationToken = default);
    Task<T?> GetEntityByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task CreateEntityAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateEntityAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteEntityAsync(Guid id, CancellationToken cancellationToken = default);
    
}