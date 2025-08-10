using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Path).IsRequired().HasMaxLength(500);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.Code);
        builder.HasIndex(x => x.Path);
        builder.HasIndex(x => x.PageGroupId);
        builder.HasIndex(x => x.ParentId);
        builder.HasIndex(x => x.DeletedAt);

        builder.HasOne(x => x.PageGroup)
            .WithMany(x => x.Pages)
            .HasForeignKey(x => x.PageGroupId);

        builder.HasMany(x => x.PagePermissions)
            .WithOne(x => x.Page)
            .HasForeignKey(x => x.PageId);
    }
}