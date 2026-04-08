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

    //Write a command to delete all user's (parameter userId) transactions in the current month
    public async Task DeleteUsersTransactionsForCurrentMonthAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.Now;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);
        
        var transactionIds = await _dbContext.Transactions
            .Join(_dbContext.Assets,
                t => t.AssetId,
                a => a.Id,
                (t, a) => new { Transaction = t, Asset = a })
            .Where(x => x.Asset.UserId == userId
                        && x.Transaction.Date >= startOfMonth
                        && x.Transaction.Date < endOfMonth)
            .Select(x => x.Transaction.Id)
            .ToListAsync(cancellationToken);

        if (transactionIds.Count != 0)
        {
            await _dbContext.Transactions
                .Where(t => transactionIds.Contains(t.Id))
                .ExecuteDeleteAsync(cancellationToken);
        }
    }

    //Write a request to get the user by email
    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync(cancellationToken);
    }

    //Write a query to get the user list sorted by the user’s name. Each record of the output model should include User.Id, User.Name and User.Email
    public async Task<List<UserInfoDto>> GetUsersSortedByNameAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.OrderBy(u => u.Name).Select(u => new UserInfoDto(u.Id, u.Name, u.Email))
            .ToListAsync(cancellationToken);
    }

    //Write a query to return the current balance for the selected user (parameter userId).
    //Each record of the output model should include User.Id, User.Email, User.Name, and Balance
    public async Task<UserBalanceDto?> GetUserBalanceAsync(Guid userId, CancellationToken cancellationToken = default)
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

    //Write a query to get the asset list for the selected user (userId) ordered by the asset’s name.
    //Each record of the output model should include Asset.Id, Asset.Name and Balance
    public async Task<List<UserAssetsDto>?> GetUserAssetsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FindAsync(userId, cancellationToken);
        if (user == null)
        {
            return null;
        }

        var userAssets = await _dbContext.Assets
            .Where(a => a.UserId == userId)
            .OrderBy(a => a.Name)
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
            .ToListAsync(cancellationToken);
        
        return userAssets;
    }

    //Write a query to return the transaction list for the selected user (userId) ordered descending by Transaction.Date, then ordered ascending by Asset.Name
    //and then ordered ascending by Category.Name. Each record of the output model should include Asset.Name, Category.Name (transaction subcategory),
    //Category.ParentName (transaction parent category), Transaction.Amount, Transaction.Date and Transaction.Comment
    public async Task<List<UserTransactionsDto>?> GetUserTransactionsAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FindAsync(userId, cancellationToken);
        if (user == null)
        {
            return null;
        }

        var userTransactions = await _dbContext.Transactions.Join(_dbContext.Assets,
                t => t.AssetId,
                a => a.Id,
                (t, a) => new { Transaction = t, Asset = a })
            .Where(t => t.Asset.UserId == userId)
            .Join(_dbContext.Categories,
                t => t.Transaction.CategoryId,
                c => c.Id,
                (t, c) => new { Transaction = t.Transaction, Asset = t.Asset, Category = c })
            .GroupJoin(_dbContext.Categories,
                t => t.Category.ParentId,
                c => c.Id,
                (t, c) => new
                    { Transaction = t.Transaction, Asset = t.Asset, Category = t.Category, ParentCategory = c.FirstOrDefault() })
            .OrderByDescending(x => x.Transaction.Date)
            .ThenBy(x => x.Asset.Name)
            .ThenBy(x => x.Category.Name)
            .Select(x => 
                new UserTransactionsDto(x.Asset.Name, x.Category.Name, x.ParentCategory != null ? x.ParentCategory.Name : null,
                    x.Transaction.Amount, x.Transaction.Date, x.Transaction.Comment))
            .ToListAsync(cancellationToken);
        
        return userTransactions;
    }
    
    //Write a query to return the total value of income and expenses for the selected period (parameters userId, startDate, endDate) ordered by Transaction.Date
    //and grouped by month. Each record of the output model should include total Income and Expenses, Month and Year.
    public async Task<List<TimePeriodIncomeExpensesDto>> GetUserTimePeriodIncomeExpensesAsync(Guid userId, DateTime startDate,
        DateTime endDate, CancellationToken cancellationToken = default)
    {
        var totalValueOfIncomeAndExpenses = await _dbContext.Transactions
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .Join(_dbContext.Assets,
                t => t.AssetId,
                a => a.Id,
                (t, a) => new { Transaction = t, Asset = a })
            .Where(x => x.Asset.UserId == userId)
            .Join(_dbContext.Categories,
                t => t.Transaction.CategoryId,
                c => c.Id,
                (t, c) => new { Transaction = t.Transaction, Asset = t.Asset, Category = c })

            .GroupBy(x => new { Year = x.Transaction.Date.Year, Month = x.Transaction.Date.Month })
            .OrderBy(x => x.Key.Year)
            .ThenBy(x => x.Key.Month)
            .Select(g => new TimePeriodIncomeExpensesDto
            (
                g.Sum(x => x.Category.Type == CategoryType.Income ? x.Transaction.Amount : 0),
                g.Sum(x => x.Category.Type == CategoryType.Expense ? x.Transaction.Amount : 0),
                g.Key.Year,
                g.Key.Month
            ))
            .ToListAsync(cancellationToken);

        return totalValueOfIncomeAndExpenses;
    }

    
    //Write a query to return the total amount of all parent categories for the selected type of operation (Income or Expenses).
    //The result should include only categories that have transactions in the current month. Input parameters in this query are UserId and
    //OperationType (category type). Each record of the output model should include Category.Name and Amount.
    //In addition, you should order results descending by Transaction.Amount and then ordered them by Category.Name
    public async Task<List<ParentCategoryTotalAmountDto>> GetUserParentCategoryTotalAmountAsync(Guid userId, CategoryType categoryType,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.Now;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);

        var unsortedParentCategoryTotalAmountQuery = _dbContext.Transactions
            .Where(t => t.Date >= startOfMonth && t.Date < endOfMonth)
            .Join(_dbContext.Assets,
                t => t.AssetId,
                a => a.Id,
                (t, a) => new { Transaction = t, Asset = a })
            .Where(x => x.Asset.UserId == userId)
            .Join(_dbContext.Categories,
                t => t.Transaction.CategoryId,
                c => c.Id,
                (t, c) => new { Transaction = t.Transaction, Asset = t.Asset, Category = c })
            .Join(_dbContext.Categories,
                t => t.Category.ParentId,
                c => c.Id,
                (t, c) => new
                    { Transaction = t.Transaction, Asset = t.Asset, Category = t.Category, ParentCategory = c })
            .Where(t => t.ParentCategory.Type == categoryType)
            .GroupBy(x => x.ParentCategory)
            .Select(g => new ParentCategoryTotalAmountDto
            (
                g.Key.Name,
                g.Sum(x => x.Transaction.Amount)
            ));
        
            var parentCategoryTotalAmount = (await unsortedParentCategoryTotalAmountQuery.ToListAsync(cancellationToken))
                .OrderByDescending(x => x.TotalAmount)
                .ThenBy(x => x.CategoryName)
                .ToList();
            
        return parentCategoryTotalAmount;
    }
}