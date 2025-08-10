using MeUi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeUi.Infrastructure.Data.Configurations;

public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.RefreshTokenId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.DeletedAt);

        builder.HasIndex(x => new { x.UserId, x.RefreshTokenId }).IsUnique();
        builder.HasIndex(x => x.DeletedAt);

        builder.HasOne(x => x.User)
            .WithMany(u => u.UserRefreshTokens)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.RefreshToken)
            .WithMany(rt => rt.UserRefreshTokens)
            .HasForeignKey(x => x.RefreshTokenId);
    }
}