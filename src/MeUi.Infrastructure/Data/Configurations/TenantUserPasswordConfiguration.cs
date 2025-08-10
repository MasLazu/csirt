using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class TenantUserPasswordConfiguration : IEntityTypeConfiguration<TenantUserPassword>
{
    public void Configure(EntityTypeBuilder<TenantUserPassword> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PasswordId).IsRequired();
        builder.Property(x => x.TenantUserLoginMethodId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.TenantUserLoginMethodId);
        builder.HasIndex(x => x.DeletedAt);

        builder.HasOne(x => x.Password)
            .WithMany(p => p.TenantUserPasswords)
            .HasForeignKey(x => x.PasswordId);

        builder.HasOne(x => x.TenantUserLoginMethod)
            .WithMany()
            .HasForeignKey(x => x.TenantUserLoginMethodId);
    }
}