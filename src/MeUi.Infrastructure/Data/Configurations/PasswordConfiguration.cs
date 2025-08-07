using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class PasswordConfiguration : IEntityTypeConfiguration<Password>
{
    public void Configure(EntityTypeBuilder<Password> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserLoginMethodId)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.PasswordSalt)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.DeletedAt);

        // Indexes
        builder.HasIndex(x => x.UserLoginMethodId)
            .IsUnique();

        builder.HasIndex(x => x.DeletedAt);

        // Relationships
        builder.HasOne(x => x.UserLoginMethod)
            .WithMany()
            .HasForeignKey(x => x.UserLoginMethodId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}