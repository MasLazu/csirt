using FluentValidation;

namespace MeUi.Application.Features.Protocols.Queries.GetProtocolsPaginated;

public class GetProtocolsPaginatedQueryValidator : AbstractValidator<GetProtocolsPaginatedQuery>
{
    public GetProtocolsPaginatedQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.SortBy).Must(BeValidSort).When(x => !string.IsNullOrWhiteSpace(x.SortBy))
            .WithMessage("SortBy must be one of: name, createdAt, updatedAt");
        RuleFor(x => x.SortDirection).Must(sd => !string.IsNullOrWhiteSpace(sd) && sd.ToLowerInvariant() is "asc" or "desc")
            .When(x => !string.IsNullOrWhiteSpace(x.SortDirection))
            .WithMessage("SortDirection must be either 'asc' or 'desc'");
    }

    private static bool BeValidSort(string? sort) => sort?.ToLowerInvariant() is "name" or "createdat" or "updatedat";
}
