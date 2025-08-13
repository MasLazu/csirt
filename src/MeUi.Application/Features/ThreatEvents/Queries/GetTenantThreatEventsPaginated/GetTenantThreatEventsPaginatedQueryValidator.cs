using FluentValidation;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventsPaginated;

public class GetTenantThreatEventsPaginatedQueryValidator : AbstractValidator<GetTenantThreatEventsPaginatedQuery>
{
    public GetTenantThreatEventsPaginatedQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("Page size must be between 1 and 1000");

        // Validate time range (max 90 days for performance on 25M+ records)
        RuleFor(x => x)
            .Must(ValidateTimeRange)
            .WithMessage("Time range cannot exceed 90 days for performance reasons")
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue);

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("End time must be after start time")
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue);

        RuleFor(x => x.SourcePort)
            .InclusiveBetween(1, 65535)
            .When(x => x.SourcePort.HasValue)
            .WithMessage("Source port must be between 1 and 65535");

        RuleFor(x => x.DestinationPort)
            .InclusiveBetween(1, 65535)
            .When(x => x.DestinationPort.HasValue)
            .WithMessage("Destination port must be between 1 and 65535");

        RuleFor(x => x.Category)
            .Length(2, 50)
            .When(x => !string.IsNullOrWhiteSpace(x.Category))
            .WithMessage("Category must be between 2 and 50 characters");

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Search))
            .WithMessage("Search term must not exceed 100 characters");
    }

    private static bool ValidateTimeRange(GetTenantThreatEventsPaginatedQuery query)
    {
        if (!query.StartTime.HasValue || !query.EndTime.HasValue)
        {
            return true;
        }

        TimeSpan timeSpan = query.EndTime.Value - query.StartTime.Value;
        return timeSpan.TotalDays <= 90;
    }
}
