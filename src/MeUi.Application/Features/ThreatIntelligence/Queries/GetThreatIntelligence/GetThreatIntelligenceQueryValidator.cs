using FluentValidation;
using System.Net;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatIntelligence;

public class GetThreatIntelligenceQueryValidator : AbstractValidator<GetThreatIntelligenceQuery>
{
    private static readonly string[] ValidSortFields =
    {
        "timestamp", "asn", "category", "sourceaddress", "sourcecountry",
        "destinationaddress", "destinationcountry", "protocol", "sourceport",
        "destinationport", "family"
    };

    public GetThreatIntelligenceQueryValidator()
    {
        // ASN validation - should be numeric or alphanumeric
        RuleFor(x => x.Asn)
            .Matches(@"^[A-Za-z0-9]+$")
            .When(x => !string.IsNullOrEmpty(x.Asn))
            .WithMessage("ASN must contain only alphanumeric characters.");

        // IP address validation for source address
        RuleFor(x => x.SourceAddress)
            .Must(BeValidIpAddress)
            .When(x => !string.IsNullOrEmpty(x.SourceAddress))
            .WithMessage("Source address must be a valid IP address.");

        // IP address validation for destination address
        RuleFor(x => x.DestinationAddress)
            .Must(BeValidIpAddress)
            .When(x => !string.IsNullOrEmpty(x.DestinationAddress))
            .WithMessage("Destination address must be a valid IP address.");

        // Country code validation - 2 character ISO codes
        RuleFor(x => x.SourceCountry)
            .Length(2)
            .Matches(@"^[A-Z]{2}$")
            .When(x => !string.IsNullOrEmpty(x.SourceCountry))
            .WithMessage("Source country must be a 2-character uppercase country code (e.g., 'US', 'ID').");

        RuleFor(x => x.DestinationCountry)
            .Length(2)
            .Matches(@"^[A-Z]{2}$")
            .When(x => !string.IsNullOrEmpty(x.DestinationCountry))
            .WithMessage("Destination country must be a 2-character uppercase country code (e.g., 'US', 'ID').");

        // Category validation - should not be empty if provided
        RuleFor(x => x.Category)
            .NotEmpty()
            .When(x => x.Category != null)
            .WithMessage("Category cannot be empty if provided.");

        // Protocol validation - common network protocols
        RuleFor(x => x.Protocol)
            .Must(BeValidProtocol)
            .When(x => !string.IsNullOrEmpty(x.Protocol))
            .WithMessage("Protocol must be a valid network protocol (TCP, UDP, ICMP, etc.).");

        // Port range validation
        RuleFor(x => x.SourcePort)
            .InclusiveBetween(1, 65535)
            .When(x => x.SourcePort.HasValue)
            .WithMessage("Source port must be between 1 and 65535.");

        RuleFor(x => x.DestinationPort)
            .InclusiveBetween(1, 65535)
            .When(x => x.DestinationPort.HasValue)
            .WithMessage("Destination port must be between 1 and 65535.");

        // Malware family validation
        RuleFor(x => x.MalwareFamily)
            .NotEmpty()
            .When(x => x.MalwareFamily != null)
            .WithMessage("Malware family cannot be empty if provided.");

        // Date range validation
        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.StartDate.HasValue)
            .WithMessage("Start date cannot be in the future.");

        RuleFor(x => x.EndDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.EndDate.HasValue)
            .WithMessage("End date cannot be in the future.");

        RuleFor(x => x)
            .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.StartDate <= x.EndDate)
            .WithMessage("Start date must be less than or equal to end date.");

        // Sort field validation
        RuleFor(x => x.SortBy)
            .Must(BeValidSortField)
            .When(x => !string.IsNullOrEmpty(x.SortBy))
            .WithMessage($"Sort field must be one of: {string.Join(", ", ValidSortFields)}.");
    }

    private static bool BeValidIpAddress(string? ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress))
            return true;

        return IPAddress.TryParse(ipAddress, out _);
    }

    private static bool BeValidProtocol(string? protocol)
    {
        if (string.IsNullOrEmpty(protocol))
            return true;

        var validProtocols = new[] { "TCP", "UDP", "ICMP", "IGMP", "GRE", "ESP", "AH", "SCTP" };
        return validProtocols.Contains(protocol.ToUpperInvariant());
    }

    private static bool BeValidSortField(string? sortField)
    {
        if (string.IsNullOrEmpty(sortField))
            return true;

        return ValidSortFields.Contains(sortField.ToLowerInvariant());
    }
}