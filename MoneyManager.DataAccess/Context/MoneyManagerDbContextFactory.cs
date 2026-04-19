using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MoneyManager.DataAccess.Context;

public class MoneyManagerDbContextFactory : IDesignTimeDbContextFactory<MoneyManagerDbContext>
{
    public MoneyManagerDbContext CreateDbContext(string[] args)
    {
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\.."));
        var configPath = Path.Combine(projectRoot, "appsettings.json");
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile(configPath)
            .Build();
        var connectionString = config.GetConnectionString("SqlServerConnectionString");
        var optionsBuilder = new DbContextOptionsBuilder<MoneyManagerDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        
        return new MoneyManagerDbContext(optionsBuilder.Options);
    }
}