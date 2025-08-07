using Microsoft.Extensions.Logging;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Seeders;

public class MongoDbSeeder
{
    private readonly IRepository<ThreatIntelligence> _threatIntelligenceRepository;
    private readonly ILogger<MongoDbSeeder> _logger;

    public MongoDbSeeder(
        IRepository<ThreatIntelligence> threatIntelligenceRepository,
        ILogger<MongoDbSeeder> logger)
    {
        _threatIntelligenceRepository = threatIntelligenceRepository ?? throw new ArgumentNullException(nameof(threatIntelligenceRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Starting MongoDB seeding");

            await SeedThreatIntelligenceDataAsync(ct);

            _logger.LogInformation("MongoDB seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during MongoDB seeding");
            throw;
        }
    }

    private async Task SeedThreatIntelligenceDataAsync(CancellationToken ct)
    {
        // Check if data already exists
        var existingCount = await _threatIntelligenceRepository.CountAsync(ct);
        if (existingCount > 0)
        {
            _logger.LogInformation("ThreatIntelligence data already exists ({Count} records), skipping seeding", existingCount);
            return;
        }

        var sampleData = new List<ThreatIntelligence>
        {
            new ThreatIntelligence
            {
                Asn = "AS13335",
                Timestamp = DateTime.UtcNow.AddDays(-1),
                AsnInfo = "CLOUDFLARENET - Cloudflare, Inc., US",
                Category = "malware",
                SourceAddress = "192.168.1.100",
                SourceCountry = "US",
                OptionalInformation = new OptionalInformation
                {
                    DestinationAddress = "10.0.0.1",
                    DestinationCountry = "CA",
                    Protocol = "TCP",
                    SourcePort = "80",
                    DestinationPort = "443",
                    Family = "trojan"
                }
            },
            new ThreatIntelligence
            {
                Asn = "AS8075",
                Timestamp = DateTime.UtcNow.AddHours(-12),
                AsnInfo = "MICROSOFT-CORP-MSN-AS-BLOCK - Microsoft Corporation, US",
                Category = "phishing",
                SourceAddress = "203.0.113.45",
                SourceCountry = "GB",
                OptionalInformation = new OptionalInformation
                {
                    DestinationAddress = "198.51.100.23",
                    DestinationCountry = "DE",
                    Protocol = "HTTP",
                    SourcePort = "8080",
                    DestinationPort = "80",
                    Family = "phishing-kit"
                }
            },
            new ThreatIntelligence
            {
                Asn = "AS15169",
                Timestamp = DateTime.UtcNow.AddHours(-6),
                AsnInfo = "GOOGLE - Google LLC, US",
                Category = "botnet",
                SourceAddress = "172.16.0.50",
                SourceCountry = "CN",
                OptionalInformation = new OptionalInformation
                {
                    DestinationAddress = "192.0.2.100",
                    DestinationCountry = "RU",
                    Protocol = "UDP",
                    SourcePort = "53",
                    DestinationPort = "53",
                    Family = "mirai"
                }
            },
            new ThreatIntelligence
            {
                Asn = "AS16509",
                Timestamp = DateTime.UtcNow.AddHours(-3),
                AsnInfo = "AMAZON-02 - Amazon.com, Inc., US",
                Category = "spam",
                SourceAddress = "198.51.100.75",
                SourceCountry = "FR",
                OptionalInformation = new OptionalInformation
                {
                    DestinationAddress = "203.0.113.150",
                    DestinationCountry = "JP",
                    Protocol = "SMTP",
                    SourcePort = "25",
                    DestinationPort = "587",
                    Family = "spam-bot"
                }
            },
            new ThreatIntelligence
            {
                Asn = "AS3356",
                Timestamp = DateTime.UtcNow.AddHours(-1),
                AsnInfo = "LEVEL3 - Level 3 Parent, LLC, US",
                Category = "c2",
                SourceAddress = "10.1.1.200",
                SourceCountry = "BR",
                OptionalInformation = new OptionalInformation
                {
                    DestinationAddress = "172.16.1.50",
                    DestinationCountry = "IN",
                    Protocol = "HTTPS",
                    SourcePort = "443",
                    DestinationPort = "8443",
                    Family = "cobalt-strike"
                }
            }
        };

        await _threatIntelligenceRepository.AddRangeAsync(sampleData, ct);
        _logger.LogInformation("Seeded {Count} ThreatIntelligence records", sampleData.Count);
    }
}