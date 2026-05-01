using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyManager.DataAccess.Entities;

namespace MoneyManager.DataAccess.Configurations;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");
        builder.HasKey(x => x.Id);
        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(64);
        builder
            .Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(64);
        builder
            .Property(x => x.Hash)
            .IsRequired()
            .HasMaxLength(1024);
        builder
            .Property(x => x.Salt)
            .IsRequired()
            .HasMaxLength(1024);
    }
}