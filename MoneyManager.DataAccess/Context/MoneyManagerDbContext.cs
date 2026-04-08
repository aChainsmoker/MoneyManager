using Microsoft.EntityFrameworkCore;
using MoneyManager.DataAccess.Entities;

namespace MoneyManager.DataAccess.Context;

public class MoneyManagerDbContext : DbContext
{
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=MoneyManagementDb;User Id=sa;Password=YourStrong@Passw0rd!;TrustServerCertificate=True;Encrypt=False");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MoneyManagerDbContext).Assembly);
        modelBuilder.Ignore<EntityBase>();
        modelBuilder.Entity<EntityBase>().UseTpcMappingStrategy();
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
}