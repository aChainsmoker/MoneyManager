using Microsoft.EntityFrameworkCore;
using MoneyManager.DataAccess.Entities;

namespace MoneyManager.DataAccess.Context;

public class MoneyManagerDbContext : DbContext
{
    public MoneyManagerDbContext(DbContextOptions<MoneyManagerDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MoneyManagerDbContext).Assembly);
        modelBuilder.Ignore<EntityBase>();
        modelBuilder
            .Entity<EntityBase>()
            .UseTpcMappingStrategy();
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
}