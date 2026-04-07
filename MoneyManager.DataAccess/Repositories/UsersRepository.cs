using Microsoft.EntityFrameworkCore;
using MoneyManager.DataAccess.Context;
using MoneyManager.DataAccess.DTOs;
using MoneyManager.DataAccess.Entities;

namespace MoneyManager.DataAccess.Repositories;

public class UsersRepository : BasicRepository<User>
{
    private readonly MoneyManagerDbContext _dbContext;

    public UsersRepository(MoneyManagerDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task DeleteUsersTransactionsForCurrentMonth(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.Now;
        var dayAMonthAgo = now.AddMonths(-1);

        var transactionIds = await _dbContext.Transactions
            .Join(_dbContext.Assets,
                t => t.AssetId,
                a => a.Id,
                (t, a) => new { Transaction = t, Asset = a })
            .Where(x => x.Asset.UserId == userId
                        && x.Transaction.Date >= dayAMonthAgo
                        && x.Transaction.Date < now)
            .Select(x => x.Transaction.Id)
            .ToListAsync(cancellationToken);

        if (transactionIds.Count != 0)
        {
            await _dbContext.Transactions
                .Where(t => transactionIds.Contains(t.Id))
                .ExecuteDeleteAsync(cancellationToken);
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<UserInfoDto>> GetUsersSortedByName(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.OrderBy(u => u.Name).Select(u => new UserInfoDto(u.Id, u.Name, u.Email))
            .ToListAsync(cancellationToken);
    }

    public async Task<UserBalanceDto?> GetUserBalance(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FindAsync(userId, cancellationToken);
        if (user == null)
        {
            return null;
        }

        var balance = await _dbContext.Transactions.Join(_dbContext.Assets, 
                t => t.AssetId, 
                a => a.Id,
                (t, a) => new { Transaction = t, Asset = a })
            .Where(a => a.Asset.UserId == user.Id)
            .Join(_dbContext.Categories, 
                t => t.Transaction.CategoryId, 
                c => c.Id,
                (t, c) => new { Transaction = t.Transaction, Category = c })
            .SumAsync(x => x.Category.Type == CategoryType.Income ? x.Transaction.Amount : -x.Transaction.Amount,
                cancellationToken);

        return new UserBalanceDto(user.Id, user.Name, user.Email, balance);
    }

    public async Task<List<UserAssetsDto>?> GetUserAssets(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FindAsync(userId, cancellationToken);
        if (user == null)
        {
            return null;
        }

        var userAssets = await _dbContext.Assets
            .Where(a => a.UserId == userId)
            .Select(a => new UserAssetsDto
            (
                a.Id,
                a.Name,
                _dbContext.Transactions
                    .Where(t => t.AssetId == a.Id)
                    .Join(_dbContext.Categories,
                        t => t.CategoryId,
                        c => c.Id,
                        (t, c) => new { t.Amount, c.Type })
                    .Sum(x => x.Type == CategoryType.Income ? x.Amount : -x.Amount)
            ))
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
        
        return userAssets;
    }

    public async Task<List<UserTransactionsDto>?> GetUserTransactions(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FindAsync(userId, cancellationToken);
        if (user == null)
        {
            return null;
        }

        var userTransactions = _dbContext.Transactions.Join(_dbContext.Assets,
                t => t.AssetId,
                a => a.Id,
                (t, a) => new { Transaction = t, Asset = a })
            .Where(t => t.Asset.UserId == userId)
            .Join(_dbContext.Categories,
                t => t.Transaction.CategoryId,
                c => c.Id,
                (t, c) => new { Transaction = t.Transaction, Asset = t.Asset, Category = c })
            .Join(_dbContext.Categories,
                t => t.Category.ParentId,
                c => c.Id,
                (t, c) => new
                    { Transaction = t.Transaction, Asset = t.Asset, Category = t.Category, ParentCategory = c })
            .OrderByDescending(x => x.Transaction.Date)
            .ThenBy(x => x.Asset.Name)
            .ThenBy(x => x.Category.Name)
            .Select(x => 
                new UserTransactionsDto(x.Asset.Name, x.Category.Name, x.ParentCategory.Name, x.Transaction.Amount, x.Transaction.Date, x.Transaction.Comment));
        
        return await userTransactions.ToListAsync(cancellationToken);
    }
}