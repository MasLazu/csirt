using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class ThreatEventConfiguration : IEntityTypeConfiguration<ThreatEvent>
{
    public void Configure(EntityTypeBuilder<ThreatEvent> builder)
    {
        builder.HasKey(e => new { e.Id, e.Timestamp });

        builder.Property(e => e.Timestamp).IsRequired();
        builder.Property(e => e.SourceAddress).IsRequired();
        builder.Property(e => e.Category).IsRequired().HasMaxLength(50);
        builder.Property(e => e.AsnRegistryId).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.HasIndex(e => e.SourceAddress);
        builder.HasIndex(e => e.AsnRegistryId);
        builder.HasIndex(e => e.DestinationAddress);
        builder.HasIndex(e => e.Category);
        builder.HasIndex(e => e.SourceCountryId);
        builder.HasIndex(e => e.DestinationCountryId);
        builder.HasIndex(e => e.DeletedAt);
        builder.HasIndex(e => e.MalwareFamilyId);
        builder.HasIndex(e => e.ProtocolId);

        builder.HasOne(e => e.AsnRegistry)
            .WithMany(a => a.ThreatEvents)
            .HasForeignKey(e => e.AsnRegistryId);

        builder.HasOne(e => e.SourceCountry)
            .WithMany(c => c.SourceThreats)
            .HasForeignKey(e => e.SourceCountryId);

        builder.HasOne(e => e.DestinationCountry)
            .WithMany(c => c.DestinationThreats)
            .HasForeignKey(e => e.DestinationCountryId);

        builder.HasOne(e => e.Protocol)
            .WithMany(p => p.ThreatEvents)
            .HasForeignKey(e => e.ProtocolId);

        builder.HasOne(e => e.MalwareFamily)
            .WithMany(m => m.ThreatEvents)
            .HasForeignKey(e => e.MalwareFamilyId);
    }
}