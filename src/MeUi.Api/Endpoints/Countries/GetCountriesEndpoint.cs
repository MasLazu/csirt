using MeUi.Api.Endpoints;
using MeUi.Application.Features.Countries.Queries.GetCountriesPaginated;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Countries;

public class GetCountriesEndpoint : BaseAuthorizedEndpoint<GetCountriesPaginatedQuery, PaginatedDto<CountryDto>, GetCountriesEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:COUNTRY";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/countries");
        Description(x => x.WithTags("Country Management")
            .WithSummary("Get paginated list of countries")
            .WithDescription("Retrieves a paginated list of countries with optional search and sorting. Requires READ:COUNTRY permission."));
    }

    public override async Task HandleAuthorizedAsync(GetCountriesPaginatedQuery req, Guid userId, CancellationToken ct)
    {
        PaginatedDto<CountryDto> countries = await Mediator.Send(req, ct);
        await SendSuccessAsync(countries, $"Retrieved {countries.Items.Count()} countries successfully", ct);
    }
}
