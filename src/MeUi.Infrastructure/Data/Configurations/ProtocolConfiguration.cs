using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class ProtocolConfiguration : IEntityTypeConfiguration<Protocol>
{
    public void Configure(EntityTypeBuilder<Protocol> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(20);
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.DeletedAt);

        builder.HasMany(e => e.ThreatEvents)
            .WithOne(t => t.Protocol)
            .HasForeignKey(t => t.ProtocolId);
    }
}