using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyManager.DataAccess.Entities;

namespace MoneyManager.DataAccess.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transaction");
        builder.HasKey(x => x.Id);
        builder
            .HasOne<Category>().WithMany()
            .HasForeignKey(x => x.CategoryId);
        builder
            .Property(x=>x.Amount)
            .IsRequired()
            .HasPrecision(16,3);
        builder
            .Property(x=>x.Date)
            .IsRequired()
            .HasPrecision(7);
        builder
            .HasOne<Asset>()
            .WithMany()
            .HasForeignKey(x => x.AssetId);
        builder
            .Property(x=>x.Comment)
            .IsRequired(false)
            .HasMaxLength(1024);
    }
}