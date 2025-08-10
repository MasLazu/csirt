using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class PageGroupConfiguration : IEntityTypeConfiguration<PageGroup>
{
    public void Configure(EntityTypeBuilder<PageGroup> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Icon).HasMaxLength(100);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.DeletedAt);

        builder.HasIndex(x => x.Code);
        builder.HasIndex(x => x.DeletedAt);

        builder.HasMany(x => x.Pages)
            .WithOne(x => x.PageGroup)
            .HasForeignKey(x => x.PageGroupId);
    }
}