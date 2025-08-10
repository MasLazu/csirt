using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class ActionConfiguration : IEntityTypeConfiguration<Domain.Entities.Action>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Action> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.Code);

        builder.HasIndex(x => x.DeletedAt);

        builder.HasMany(x => x.Permissions)
            .WithOne(x => x.Action)
            .HasForeignKey(x => x.ActionCode)
            .HasPrincipalKey(x => x.Code);

        builder.HasMany(x => x.TenantPermissions)
            .WithOne(x => x.Action)
            .HasForeignKey(x => x.ActionCode)
            .HasPrincipalKey(x => x.Code);
    }
}