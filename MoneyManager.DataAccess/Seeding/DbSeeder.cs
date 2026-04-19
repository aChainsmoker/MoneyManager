using Microsoft.EntityFrameworkCore;
using MoneyManager.DataAccess.Context;
using MoneyManager.DataAccess.Entities;
using MoneyManager.Utility.Encryption;

namespace MoneyManager.DataAccess.Seeding;

public class DbSeeder
{
    private readonly MoneyManagerDbContext _dbContext;

    public DbSeeder(MoneyManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public void SeedDb()
    {
        var random = new Random(1); 
        
        var users = new List<User>
        {
            new() 
            { 
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), 
                Email = "john.doe@example.com", 
                Name = "John Doe",
                Salt = PasswordHasher.GenerateSalt() 
            },
            new()
            { 
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), 
                Email = "alice.smith@example.com", 
                Name = "Alice Smith", 
                Salt = PasswordHasher.GenerateSalt()
            },
            new() 
            { 
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Email = "bob.johnson@example.com",
                Name = "Bob Johnson", 
                Salt = PasswordHasher.GenerateSalt() 
            },
            new() 
            { 
                Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), 
                Email = "charlie.brown@example.com", 
                Name = "Charlie Brown",
                Salt = PasswordHasher.GenerateSalt() 
            },
            new() 
            { 
                Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), 
                Email = "david.wilson@example.com",
                Name = "David Wilson", 
                Salt = PasswordHasher.GenerateSalt() 
            },
            new() 
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), 
                Email = "emma.davis@example.com", 
                Name = "Emma Davis", 
                Salt = PasswordHasher.GenerateSalt() 
            }
        };
        for (int i =0; i < users.Count; i++)
        {
            users[i].Hash = PasswordHasher.HashPassword($"password{i}", users[i].Salt);
        }
        
        var assets = new List<Asset>
        {
            new() 
            {
                Id = Guid.NewGuid(), 
                Name = "Cash", 
                UserId = users[0].Id
            },
            new()
            { 
                Id = Guid.NewGuid(), 
                Name = "Debit Card", 
                UserId = users[0].Id 
            },
            new()
            { 
                Id = Guid.NewGuid(), 
                Name = "Savings Account", 
                UserId = users[0].Id 
            },
            new()
            {
                Id = Guid.NewGuid(), 
                Name = "Credit Card", 
                UserId = users[0].Id
            },
            
            new() 
            {
                Id = Guid.NewGuid(), 
                Name = "Cash",
                UserId = users[1].Id
            },
            new()
            {
                Id = Guid.NewGuid(), 
                Name = "Bank Card", 
                UserId = users[1].Id
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                Name = "Investment Account",
                UserId = users[1].Id
            },
            
            new()
            {
                Id = Guid.NewGuid(), 
                Name = "Wallet",
                UserId = users[2].Id 
            },
            new() 
            {
                Id = Guid.NewGuid(),
                Name = "Checking Account",
                UserId = users[2].Id
            },
            new()
            {
                Id = Guid.NewGuid(), 
                Name = "Savings",
                UserId = users[2].Id 
            },
            
            new() 
            { 
                Id = Guid.NewGuid(), 
                Name = "Cash",
                UserId = users[3].Id 
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                Name = "Credit Card",
                UserId = users[3].Id
            },
            new()
            { 
                Id = Guid.NewGuid(),
                Name = "PayPal", 
                UserId = users[3].Id
            },
            
            new() 
            {
                Id = Guid.NewGuid(), 
                Name = "Cash",
                UserId = users[4].Id 
            },
            new() 
            {
                Id = Guid.NewGuid(),
                Name = "Debit Card",
                UserId = users[4].Id
            },
            new() 
            { 
                Id = Guid.NewGuid(),
                Name = "Bank Account", 
                UserId = users[4].Id 
            },
            
            new() 
            {
                Id = Guid.NewGuid(), 
                Name = "Cash", 
                UserId = users[5].Id
            },
            new()
            { 
                Id = Guid.NewGuid(),
                Name = "Credit Card", 
                UserId = users[5].Id
            },
            new()
            { 
                Id = Guid.NewGuid(), 
                Name = "Savings Account", 
                UserId = users[5].Id 
            },
            new() 
            { 
                Id = Guid.NewGuid(),
                Name = "Crypto Wallet", 
                UserId = users[5].Id
            },
            
            new()
            { 
                Id = Guid.NewGuid(),
                Name = "Loan", 
                UserId = users[0].Id 
            }
        };
        
        var categories = new List<Category>
        {
            new()
            { 
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Name = "Food", 
                Type = CategoryType.Expense,
                ParentId = null
            },
            new() 
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Name = "Transportation", 
                Type = CategoryType.Expense, ParentId = null
            },
            new() 
            { 
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), 
                Name = "Social Life", 
                Type = CategoryType.Expense, 
                ParentId = null 
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), 
                Name = "Culture", 
                Type = CategoryType.Expense, 
                ParentId = null
            },
            new() 
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), 
                Name = "Self-development", 
                Type = CategoryType.Expense, 
                ParentId = null 
            },
            
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000101"), 
                Name = "Groceries", 
                Type = CategoryType.Expense, 
                ParentId = Guid.Parse("10000000-0000-0000-0000-000000000001") 
            },
            new() 
            { 
                Id = Guid.Parse("10000000-0000-0000-0000-000000000102"), 
                Name = "Restaurants", 
                Type = CategoryType.Expense, 
                ParentId = Guid.Parse("10000000-0000-0000-0000-000000000001") 
            },
            new() 
            { 
                Id = Guid.Parse("10000000-0000-0000-0000-000000000103"), 
                Name = "Taxi", 
                Type = CategoryType.Expense, 
                ParentId = Guid.Parse("10000000-0000-0000-0000-000000000002")
            },
            new() 
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000104"), 
                Name = "Public Transport", Type = CategoryType.
                Expense, ParentId = Guid.Parse("10000000-0000-0000-0000-000000000002") 
            },
            new() {
                Id
                    = Guid.Parse("10000000-0000-0000-0000-000000000105"), 
                Name = "Parking", 
                Type = CategoryType.Expense, 
                ParentId = Guid.Parse("10000000-0000-0000-0000-000000000002")
            },
            new()
            { 
                Id = Guid.Parse("10000000-0000-0000-0000-000000000106"), 
                Name = "Cinema", 
                Type = CategoryType.Expense, 
                ParentId = Guid.Parse("10000000-0000-0000-0000-000000000004")
            },
            new() { 
                Id = Guid.Parse("10000000-0000-0000-0000-000000000107"), 
                Name = "Courses", 
                Type = CategoryType.Expense, 
                ParentId = Guid.Parse("10000000-0000-0000-0000-000000000005")
            },
            
            new() 
            { 
                Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), 
                Name = "Salary", 
                Type = CategoryType.Income, 
                ParentId = null
            },
            new() 
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), 
                Name = "Bonus", 
                Type = CategoryType.Income, 
                ParentId = null
            },
            new() 
            { 
                Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), 
                Name = "Petty Cash", 
                Type = CategoryType.Income, 
                ParentId = null 
            },
            new()
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000004"), 
                Name = "Freelance", 
                Type = CategoryType.Income,
                ParentId = null
            },
            
            new()
            { 
                Id = Guid.Parse("20000000-0000-0000-0000-000000000101"), 
                Name = "Monthly Salary", 
                Type = CategoryType.Income, 
                ParentId = Guid.Parse("20000000-0000-0000-0000-000000000001")
            },
            new()
            { 
                Id = Guid.Parse("20000000-0000-0000-0000-000000000102"), 
                Name = "Quarterly Bonus", 
                Type = CategoryType.Income, 
                ParentId = Guid.Parse("20000000-0000-0000-0000-000000000002") 
            },
            new() 
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000103"), 
                Name = "Project Bonus", 
                Type = CategoryType.Income, 
                ParentId = Guid.Parse("20000000-0000-0000-0000-000000000002") 
            },
            new()
            { 
                Id = Guid.Parse("20000000-0000-0000-0000-000000000104"), 
                Name = "Side Projects", 
                Type = CategoryType.Income,
                ParentId = Guid.Parse("20000000-0000-0000-0000-000000000004")
            }
        };
        
        var transactions = new List<Transaction>();
        var now = DateTime.Now;
        var comments = new[] { "Monthly payment", "Regular expense", "One-time purchase", "Subscription", "Cash withdrawal", "Online payment", null };
        
        for (int i = 0; i < 110; i++)
        {
            var userIndex = i % users.Count;
            var userAssets = assets
                .Where(a => a.UserId == users[userIndex].Id)
                .ToList();
            var asset = userAssets[random.Next(userAssets.Count)];
            
            Category category;
            decimal amount;
            
            if (i % 3 == 0) 
            {
                var incomeCategories = categories
                    .Where(c => c.Type == CategoryType.Income)
                    .ToList();
                category = incomeCategories[random.Next(incomeCategories.Count)];
                amount = (decimal)Math.Round(random.NextSingle()*1000, 3);
            }
            else 
            {
                var expenseCategories = categories
                    .Where(c => c.Type == CategoryType.Expense)
                    .ToList();
                category = expenseCategories[random.Next(expenseCategories.Count)];
                amount = (decimal)Math.Round(random.NextSingle()*1000, 3);
            }
            
            var daysAgo = random.Next(0, 90);
            var date = now
                .AddDays(-daysAgo).AddHours(random.Next(8, 20))
                .AddMinutes(random.Next(0, 60));

            transactions.Add(new Transaction
            {
                Id = Guid.NewGuid(),
                CategoryId = category.Id,
                AssetId = asset.Id,
                Amount = amount,
                Date = date,
                Comment = comments[random.Next(comments.Length)]
            });
        }
        
        using var transaction = _dbContext.Database.BeginTransaction();
        try
        {
            _dbContext.Users.AddRange(users);
            _dbContext.Assets.AddRange(assets);
            _dbContext.Categories.AddRange(categories);
            _dbContext.Transactions.AddRange(transactions);
            
            _dbContext.SaveChanges();
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}