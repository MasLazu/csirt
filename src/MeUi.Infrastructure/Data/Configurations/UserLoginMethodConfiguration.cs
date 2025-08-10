using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class UserLoginMethodConfiguration : IEntityTypeConfiguration<UserLoginMethod>
{
    public void Configure(EntityTypeBuilder<UserLoginMethod> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.LoginMethodCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => new { x.UserId, x.LoginMethodCode }).IsUnique();
        builder.HasIndex(x => x.DeletedAt);

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserLoginMethods)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.LoginMethod)
            .WithMany(x => x.UserLoginMethods)
            .HasForeignKey(x => x.LoginMethodCode);
    }
}