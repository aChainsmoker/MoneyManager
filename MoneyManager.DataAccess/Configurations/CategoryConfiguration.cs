using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyManager.DataAccess.Entities;

namespace MoneyManager.DataAccess.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Category");
        builder
            .Property(x => x.Name)
            .IsRequired();
        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(64);
        builder
            .Property(x => x.Type)
            .IsRequired();
        builder
            .Property(x => x.ParentId)
            .IsRequired(false);
        builder
            .Property(x => x.Color)
            .IsRequired()
            .HasDefaultValue(2309453);
        builder
            .HasOne(x => x.Parent)
            .WithMany()
            .HasForeignKey(x => x.ParentId);
    }
}