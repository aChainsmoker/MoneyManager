using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyManager.DataAccess.Entities;

namespace MoneyManager.DataAccess.Configurations;

public class AssetConfigurations: IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Asset");
        builder.HasKey(x => x.Id);
        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(64);
        builder
            .HasOne(x=>x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }
}