using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class TenantUserLoginMethodConfiguration : IEntityTypeConfiguration<TenantUserLoginMethod>
{
    public void Configure(EntityTypeBuilder<TenantUserLoginMethod> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantUserId).IsRequired();
        builder.Property(x => x.LoginMethodCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => new { x.TenantUserId, x.LoginMethodCode }).IsUnique();
        builder.HasIndex(x => x.TenantUserId);
        builder.HasIndex(x => x.LoginMethodCode);
        builder.HasIndex(x => x.DeletedAt);

        builder.HasOne(x => x.TenantUser)
            .WithMany(x => x.TenantUserLoginMethods)
            .HasForeignKey(x => x.TenantUserId);

        builder.HasOne(x => x.LoginMethod)
            .WithMany()
            .HasForeignKey(x => x.LoginMethodCode);
    }
}