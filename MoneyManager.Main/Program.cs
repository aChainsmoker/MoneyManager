using MoneyManager.DataAccess.Context;
using MoneyManager.DataAccess.Entities;
using MoneyManager.DataAccess.Repositories;
using MoneyManager.DataAccess.Repositories.Abstractions;
using MoneyManager.DataAccess.Seeding;

namespace MoneyManager.Main;

class Program
{
    static async Task Main(string[] args)
    {
        using var dbContext = new MoneyManagerDbContext("Server=localhost,1433;Database=master;User Id=sa;Password=YourStrong@Passw0rd!;TrustServerCertificate=True;Encrypt=False;");
        var dbSeeder = new DbSeeder(dbContext);
        if (dbContext.Database.EnsureCreated() || 
            (!dbContext.Users.Any() && !dbContext.Categories.Any() && !dbContext.Transactions.Any() && !dbContext.Assets.Any()))
        {
            dbSeeder.SeedDb();
        }
        
        var userRepository = new UsersRepository(dbContext);
        // userRepository.DeleteUsersTransactionsForCurrentMonth(new Guid("10000000-0000-0000-0000-000000000001"));

        var userByEmail = await userRepository.GetUserByEmailAsync("david.wilson@example.com");
        Console.WriteLine(userByEmail?.Name);
        
        
    }
}