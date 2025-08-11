using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class LoginMethodConfiguration : IEntityTypeConfiguration<LoginMethod>
{
    public void Configure(EntityTypeBuilder<LoginMethod> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.DeletedAt);

        builder.HasIndex(x => x.Code);
        builder.HasIndex(x => x.DeletedAt);

        builder.HasMany(x => x.UserLoginMethods)
            .WithOne(x => x.LoginMethod)
            .HasForeignKey(x => x.LoginMethodCode)
            .HasPrincipalKey(x => x.Code);

        builder.HasMany(x => x.TenantUserLoginMethods)
            .WithOne(x => x.LoginMethod)
            .HasForeignKey(x => x.LoginMethodCode)
            .HasPrincipalKey(x => x.Code);
    }
}