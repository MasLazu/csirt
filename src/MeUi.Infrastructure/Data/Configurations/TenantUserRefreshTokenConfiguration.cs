using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class TenantUserRefreshTokenConfiguration : IEntityTypeConfiguration<TenantUserRefreshToken>
{
    public void Configure(EntityTypeBuilder<TenantUserRefreshToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantUserId).IsRequired();
        builder.Property(x => x.RefreshTokenId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => new { x.TenantUserId, x.RefreshTokenId }).IsUnique();
        builder.HasIndex(x => x.DeletedAt);

        builder.HasOne(x => x.TenantUser)
            .WithMany(tu => tu.TenantUserRefreshTokens)
            .HasForeignKey(x => x.TenantUserId);

        builder.HasOne(x => x.RefreshToken)
            .WithMany(rt => rt.TenantUserRefreshTokens)
            .HasForeignKey(x => x.RefreshTokenId);
    }
}