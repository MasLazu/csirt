using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Code).IsRequired().HasMaxLength(2);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.HasIndex(e => e.Code);
        builder.HasIndex(x => x.DeletedAt);

        builder.HasMany(e => e.SourceThreats)
            .WithOne(t => t.SourceCountry)
            .HasForeignKey(t => t.SourceCountryId);

        builder.HasMany(e => e.DestinationThreats)
            .WithOne(t => t.DestinationCountry)
            .HasForeignKey(t => t.DestinationCountryId);
    }
}