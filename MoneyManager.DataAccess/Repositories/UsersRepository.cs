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

        await _dbContext.Transactions
            .Where(t => t.Asset.UserId == userId
                       && t.Date >= startOfMonth
                       && t.Date < endOfMonth)
            .ExecuteDeleteAsync(cancellationToken);
    }

    //Write a request to get the user by email
    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    //Write a query to get the user list sorted by the user’s name. Each record of the output model should include User.Id, User.Name and User.Email
    public async Task<List<UserInfoDto>> GetUsersSortedByNameAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .OrderBy(u => u.Name)
            .Select(u => new UserInfoDto(u.Id, u.Name, u.Email))
            .ToListAsync(cancellationToken);
    }

    //Write a query to return the current balance for the selected user (parameter userId).
    //Each record of the output model should include User.Id, User.Email, User.Name, and Balance
    public async Task<UserBalanceDto?> GetUserBalanceAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FindAsync([userId], cancellationToken);
        if (user == null)
        {
            return null;
        }

        var balance = await _dbContext.Transactions
            .Where(t => t.Asset.UserId == userId)
            .SumAsync(t => t.Category.Type == CategoryType.Income ? t.Amount : -t.Amount, cancellationToken);

        return new UserBalanceDto(user.Id, user.Name, user.Email, balance);
    }

    //Write a query to get the asset list for the selected user (userId) ordered by the asset's name.
    //Each record of the output model should include Asset.Id, Asset.Name and Balance
    public async Task<List<UserAssetsDto>?> GetUserAssetsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FindAsync([userId], cancellationToken);
        if (user == null)
        {
            return null;
        }

        var userAssets = await _dbContext.Transactions
            .Where(t => t.Asset.UserId == userId)
            .GroupBy(t => new { t.Asset.Id, t.Asset.Name })
            .Select(g => new
            {
                AssetId = g.Key.Id,
                AssetName = g.Key.Name,
                Balance = g.Sum(t => t.Category.Type == CategoryType.Income ? t.Amount : -t.Amount)
            })
            .OrderBy(dto => dto.AssetName)
            .Select(a=>new UserAssetsDto(a.AssetId, a.AssetName, a.Balance))
            .ToListAsync(cancellationToken);

        return userAssets;
    }

    //Write a query to return the transaction list for the selected user (userId) ordered descending by Transaction.Date, then ordered ascending by Asset.Name
    //and then ordered ascending by Category.Name. Each record of the output model should include Asset.Name, Category.Name (transaction subcategory),
    //Category.ParentName (transaction parent category), Transaction.Amount, Transaction.Date and Transaction.Comment
    public async Task<List<UserTransactionsDto>?> GetUserTransactionsAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FindAsync([userId], cancellationToken);
        if (user == null)
        {
            return null;
        }

        var userTransactions = await _dbContext.Transactions
            .Where(t => t.Asset.UserId == userId)
            .Select(t => new 
            {
                AssetName = t.Asset.Name,
                SubcategoryName = t.Category.Name,
                ParentCategoryName = t.Category.Parent != null ? t.Category.Parent.Name : null,
                Amount = t.Amount,
                Date = t.Date,
                Comment = t.Comment
            })
            .OrderByDescending(x => x.Date)
            .ThenBy(x => x.AssetName)
            .ThenBy(x => x.SubcategoryName)
            .Select(x=> new UserTransactionsDto
                (
                    x.AssetName, 
                    x.SubcategoryName, 
                    x.ParentCategoryName, 
                    x.Amount, 
                    x.Date, 
                    x.Comment
                ))
            .ToListAsync(cancellationToken);
        
        return userTransactions;
    }
    
    //Write a query to return the total value of income and expenses for the selected period (parameters userId, startDate, endDate) ordered by Transaction.Date
    //and grouped by month. Each record of the output model should include total Income and Expenses, Month and Year.
    public async Task<List<TimePeriodIncomeExpensesDto>> GetUserTimePeriodIncomeExpensesAsync(Guid userId, DateTime startDate,
        DateTime endDate, CancellationToken cancellationToken = default)
    {
        var totalValueOfIncomeAndExpenses = await _dbContext.Transactions
            .Where(t => t.Date >= startDate && t.Date <= endDate && t.Asset.UserId == userId)
            .GroupBy(t => new { Year = t.Date.Year, Month = t.Date.Month })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .Select(g => new TimePeriodIncomeExpensesDto(
                g.Sum(t => t.Category.Type == CategoryType.Income ? t.Amount : 0),
                g.Sum(t => t.Category.Type == CategoryType.Expense ? t.Amount : 0),
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

        var parentCategoryTotalAmount = await _dbContext.Transactions
            .Where(t => t.Date >= startOfMonth 
                        && t.Date < endOfMonth 
                        && t.Asset.UserId == userId
                        && t.Category.Parent != null
                        && t.Category.Parent.Type == categoryType)
            .GroupBy(t => new { t.Category.Parent!.Id, t.Category.Parent!.Name })
            .Select(g => new 
            {
                CategoryName = g.Key.Name,
                TotalAmount = g.Sum(t => t.Amount)
            })
            .OrderByDescending(dto => dto.TotalAmount)
            .ThenBy(dto => dto.CategoryName)
            .Select(x=>new ParentCategoryTotalAmountDto(x.CategoryName, x.TotalAmount))
            .ToListAsync(cancellationToken);
        
        return parentCategoryTotalAmount;
    }
}