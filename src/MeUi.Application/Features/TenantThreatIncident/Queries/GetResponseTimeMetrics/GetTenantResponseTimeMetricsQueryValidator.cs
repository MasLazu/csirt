using FluentValidation;

namespace MeUi.Application.Features.TenantThreatIncident.Queries.GetResponseTimeMetrics;

public class GetTenantResponseTimeMetricsQueryValidator : AbstractValidator<GetTenantResponseTimeMetricsQuery>
{
    public GetTenantResponseTimeMetricsQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.Start)
            .NotEmpty()
            .WithMessage("Start date is required.")
            .LessThan(x => x.End)
            .WithMessage("Start date must be before end date.");

        RuleFor(x => x.End)
            .NotEmpty()
            .WithMessage("End date is required.")
            .GreaterThan(x => x.Start)
            .WithMessage("End date must be after start date.");

        RuleFor(x => x.Interval)
            .Must(x => x.TotalMinutes >= 1)
            .WithMessage("Interval must be at least 1 minute.");
    }
}