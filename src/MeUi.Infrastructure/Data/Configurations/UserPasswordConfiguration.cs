using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class UserPasswordConfiguration : IEntityTypeConfiguration<UserPassword>
{
    public void Configure(EntityTypeBuilder<UserPassword> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PasswordId).IsRequired();
        builder.Property(x => x.UserLoginMethodId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();


        builder.HasIndex(x => x.DeletedAt);

        builder.HasOne(x => x.Password)
            .WithMany(p => p.UserPasswords)
            .HasForeignKey(x => x.PasswordId);

        builder.HasOne(x => x.UserLoginMethod)
            .WithMany()
            .HasForeignKey(x => x.UserLoginMethodId);
    }
}