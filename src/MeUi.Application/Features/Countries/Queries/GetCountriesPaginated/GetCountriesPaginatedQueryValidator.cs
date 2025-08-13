using FluentValidation;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Countries.Queries.GetCountriesPaginated;

public class GetCountriesPaginatedQueryValidator : AbstractValidator<GetCountriesPaginatedQuery>
{
    public GetCountriesPaginatedQueryValidator()
    {
        Include(new BasePaginatedQueryValidator<CountryDto>());

        RuleFor(x => x.SortBy)
            .Must(sortBy => string.IsNullOrEmpty(sortBy) ||
                           new[] { "code", "name", "createdat", "updatedat" }.Contains(sortBy.ToLowerInvariant()))
            .WithMessage("SortBy must be one of: Code, Name, CreatedAt, UpdatedAt");
    }
}
