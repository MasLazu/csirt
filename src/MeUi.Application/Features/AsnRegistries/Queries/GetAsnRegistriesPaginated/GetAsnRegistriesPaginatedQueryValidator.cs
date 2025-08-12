using FluentValidation;
using MeUi.Application.Models;

namespace MeUi.Application.Features.AsnRegistries.Queries.GetAsnRegistriesPaginated;

public class GetAsnRegistriesPaginatedQueryValidator : AbstractValidator<GetAsnRegistriesPaginatedQuery>
{
    public GetAsnRegistriesPaginatedQueryValidator()
    {
        Include(new BasePaginatedQueryValidator<AsnRegistryDto>());

        // Valid sort fields for ASN registries - this would need to be implemented in the handler
        // ValidSortFields = new[] { "Number", "Description", "CreatedAt", "UpdatedAt" };
    }
}
