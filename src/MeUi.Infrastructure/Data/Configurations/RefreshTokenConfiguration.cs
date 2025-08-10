using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token).IsRequired().HasMaxLength(500);
        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.Token).IsUnique();
        builder.HasIndex(x => x.ExpiresAt);
        builder.HasIndex(x => x.DeletedAt);

        builder.HasMany(x => x.UserRefreshTokens)
            .WithOne(x => x.RefreshToken)
            .HasForeignKey(x => x.RefreshTokenId);

        builder.HasMany(x => x.TenantUserRefreshTokens)
            .WithOne(x => x.RefreshToken)
            .HasForeignKey(x => x.RefreshTokenId);
    }
}