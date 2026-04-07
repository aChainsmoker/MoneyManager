using Microsoft.EntityFrameworkCore;
using MoneyManager.DataAccess.Context;
using MoneyManager.DataAccess.Entities;
using MoneyManager.DataAccess.Repositories.Abstractions;

namespace MoneyManager.DataAccess.Repositories;

public class BasicRepository<T> : IBasicRepository<T> where T : EntityBase
{
    private readonly MoneyManagerDbContext _dbContext;

    public BasicRepository(MoneyManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<T>> GetAllEntitiesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>().ToListAsync(cancellationToken);
    }

    public async Task<T?> GetEntityByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>().FindAsync(id, cancellationToken);
    }

    public async Task CreateEntityAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateEntityAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteEntityAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().Where(e=>e.Id == id).ExecuteDeleteAsync(cancellationToken);
    }
}