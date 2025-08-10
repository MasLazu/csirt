using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class PagePermissionConfiguration : IEntityTypeConfiguration<PagePermission>
{
    public void Configure(EntityTypeBuilder<PagePermission> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.DeletedAt);

        builder.HasIndex(x => x.PageId);
        builder.HasIndex(x => x.PermissionId);
        builder.HasIndex(x => x.DeletedAt);

        builder.HasOne(x => x.Page)
            .WithMany(x => x.PagePermissions)
            .HasForeignKey(x => x.PageId);

        builder.HasOne(x => x.Permission)
            .WithMany(x => x.PagePermissions)
            .HasForeignKey(x => x.PermissionId);
    }
}