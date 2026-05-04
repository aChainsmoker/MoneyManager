using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoneyManager.DataAccess.Context;
using MoneyManager.DataAccess.Entities;
using MoneyManager.DataAccess.Repositories;
using MoneyManager.DataAccess.Seeding;
using MoneyManager.Utility.Encryption;

namespace MoneyManager.Main;

class Program
{
    static async Task Main(string[] args)
    {
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\.."));
        var configPath = Path.Combine(projectRoot, "appsettings.json");
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile(configPath)
            .Build();
        var connectionString = config.GetConnectionString("SqlServerConnectionString");
        var optionsBuilder = new DbContextOptionsBuilder<MoneyManagerDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        
        using var dbContext = new MoneyManagerDbContext(optionsBuilder.Options);
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
        var dbSeeder = new DbSeeder(dbContext);
        if (!dbContext.Users.Any() && !dbContext.Categories.Any() && !dbContext.Transactions.Any() &&
            !dbContext.Assets.Any())
        {
            dbSeeder.SeedDb();
        }

        var userId = new Guid("10000000-0000-0000-0000-000000000001");
        var userId2 = new Guid("10000000-0000-0000-0000-000000000002");

        var userRepository = new UsersRepository(dbContext);

        Console.WriteLine("User with email david.wilson@example.com:");
        var userByEmail = await userRepository.GetUserByEmailAsync("david.wilson@example.com");
        Console.WriteLine(userByEmail?.Name);

        Console.WriteLine("Users sorted by name:");
        var userSortedByName = await userRepository.GetUsersSortedByNameAsync();
        foreach (var user in userSortedByName)
        {
            Console.WriteLine(user);
        }

        Console.WriteLine("User's balance:");
        var userBalance = await userRepository.GetUserBalanceAsync(userId);
        Console.WriteLine(userBalance);


        Console.WriteLine("User's assets:");
        var userAssets = await userRepository.GetUserAssetsAsync(userId);
        foreach (var userAsset in userAssets)
        {
            Console.WriteLine(userAsset);
        }

        Console.WriteLine("Monthly income and expenses:");
        var monthlyIncomeAndExpenses = await userRepository.GetUserTimePeriodIncomeExpensesAsync(userId2,
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddYears(-1), DateTime.Now);
        foreach (var record in monthlyIncomeAndExpenses)
        {
            Console.WriteLine(record);
        }

        Console.WriteLine("User's category total amount:");
        var userParentCategoryTotalAmount =
            await userRepository.GetUserParentCategoryTotalAmountAsync(userId2, CategoryType.Expense);
        foreach (var record in userParentCategoryTotalAmount)
        {
            Console.WriteLine(record);
        }

        Console.WriteLine("User's transactions:");
        var usersTransactions = await userRepository.GetUserTransactionsAsync(userId);
        foreach (var transaction in usersTransactions)
        {
            Console.WriteLine(transaction);
        }

        Console.WriteLine("Amount of transactions in Db: " + dbContext.Transactions.Count());

        await userRepository.DeleteUsersTransactionsForCurrentMonthAsync(userId);
        Console.WriteLine("Amount of transactions in Db after deleting user's transactions for the current month: " +
                          dbContext.Transactions.Count());

        //Basic user testing
        Console.WriteLine("\n\n\n");
        var newUser = new User()
        {
            Id = new Guid("19000000-0000-0000-0000-000000000009"),
            Name = "New User",
            Email = "new.user@example.com",
            Salt = PasswordHasher.GenerateSalt()
        };
        newUser.Hash = PasswordHasher.HashPassword("password123", newUser.Salt);
        Console.WriteLine("User amount before adding a user: " + dbContext.Users.Count());
        await userRepository.CreateEntityAsync(newUser);
        Console.WriteLine("User amount after adding a user: " + dbContext.Users.Count());
        Console.WriteLine("All users:");
        var allUsers = await userRepository.GetAllEntitiesAsync();
        foreach (var user in allUsers)
        {
            Console.WriteLine(user.Name);
        }

        var retrievedUser = await userRepository.GetEntityByIdAsync(newUser.Id);
        Console.WriteLine("Added user: " + retrievedUser?.Name);
        newUser.Name = "Updated User";
        await userRepository.UpdateEntityAsync(newUser);
        retrievedUser = await userRepository.GetEntityByIdAsync(newUser.Id);
        Console.WriteLine("Updated user: " + retrievedUser?.Name);
        await userRepository.DeleteEntityAsync(newUser.Id);
        Console.WriteLine("User amount after deleting a user: " + dbContext.Users.Count());

        //Asset testing
        Console.WriteLine("\n\n\n");
        var assetRepository = new BasicRepository<Asset>(dbContext);
        Console.WriteLine("Assets amount before adding: " + dbContext.Assets.Count());
        var newAsset = new Asset
        {
            Id = Guid.NewGuid(),
            Name = "New Asset",
            UserId = userId
        };
        await assetRepository.CreateEntityAsync(newAsset);
        Console.WriteLine("Assets amount after adding: " + dbContext.Assets.Count());
        Console.WriteLine("All assets for user:");
        var allAssets = await assetRepository.GetAllEntitiesAsync();
        foreach (var asset in allAssets.Where(a => a.UserId == userId))
        {
            Console.WriteLine(asset.Name);
        }
        var retrievedAsset = await assetRepository.GetEntityByIdAsync(newAsset.Id);
        Console.WriteLine("Added asset: " + retrievedAsset?.Name);
        newAsset.Name = "Updated Asset";
        await assetRepository.UpdateEntityAsync(newAsset);
        retrievedAsset = await assetRepository.GetEntityByIdAsync(newAsset.Id);
        Console.WriteLine("Updated asset: " + retrievedAsset?.Name);
        await assetRepository.DeleteEntityAsync(newAsset.Id);
        Console.WriteLine("Assets amount after deleting: " + dbContext.Assets.Count());

        //Category testing
        Console.WriteLine("\n\n\n");
        var categoryRepository = new BasicRepository<Category>(dbContext);
        Console.WriteLine("Categories amount before adding: " + dbContext.Categories.Count());
        var newCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "New Category",
            Type = CategoryType.Expense,
            ParentId = null
        };
        await categoryRepository.CreateEntityAsync(newCategory);
        Console.WriteLine("Categories amount after adding: " + dbContext.Categories.Count());
        Console.WriteLine("All categories:");
        var allCategories = await categoryRepository.GetAllEntitiesAsync();
        foreach (var category in allCategories)
        {
            Console.WriteLine(category.Name);
        }

        var retrievedCategory = await categoryRepository.GetEntityByIdAsync(newCategory.Id);
        Console.WriteLine("Added category: " + retrievedCategory?.Name);
        newCategory.Name = "Updated Category";
        await categoryRepository.UpdateEntityAsync(newCategory);
        retrievedCategory = await categoryRepository.GetEntityByIdAsync(newCategory.Id);
        Console.WriteLine("Updated category:" + retrievedCategory?.Name);
        await categoryRepository.DeleteEntityAsync(newCategory.Id);
        Console.WriteLine("Categories amount after deleting: " + dbContext.Categories.Count());

        //Transactions testing
        Console.WriteLine("\n\n\n");
        var transactionRepository = new BasicRepository<Transaction>(dbContext);
        var firstAsset = dbContext.Assets.FirstOrDefault(a => a.UserId == userId);
        var firstCategory = dbContext.Categories.FirstOrDefault();
        if (firstAsset != null && firstCategory != null)
        {
            Console.WriteLine("Transactions amount before adding: " + dbContext.Transactions.Count());
            var newTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                CategoryId = firstCategory.Id,
                AssetId = firstAsset.Id,
                Amount = 999.99m,
                Date = DateTime.Now,
                Comment = "New transaction"
            };
            await transactionRepository.CreateEntityAsync(newTransaction);
            Console.WriteLine("Transactions amount after adding: " + dbContext.Transactions.Count());
            Console.WriteLine("All transactions for user:");
            var allTransactions = await transactionRepository.GetAllEntitiesAsync();
            foreach (var transaction in allTransactions)
            {
                Console.WriteLine(transaction.Id + ", " + transaction.Amount + ", " + transaction.Comment);
            }

            var retrievedTransaction = await transactionRepository.GetEntityByIdAsync(newTransaction.Id);
            Console.WriteLine(
                $"Added transaction: {retrievedTransaction?.Id}, {retrievedTransaction?.Amount}, {retrievedTransaction?.Comment}");
            newTransaction.Amount = 111.11m;
            newTransaction.Comment = "Updated transaction";
            await transactionRepository.UpdateEntityAsync(newTransaction);
            retrievedTransaction = await transactionRepository.GetEntityByIdAsync(newTransaction.Id);
            Console.WriteLine(
                $"Updated transaction: {retrievedTransaction?.Id}, {retrievedTransaction?.Amount}, {retrievedTransaction?.Comment}");
            await transactionRepository.DeleteEntityAsync(newTransaction.Id);
            Console.WriteLine("Transactions amount after deleting:" + dbContext.Transactions.Count());
        }
    }
}