using MeUi.Api.Endpoints;
using MeUi.Application.Features.Countries.Queries.GetCountry;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Countries;

public class GetCountryByIdEndpoint : BaseEndpoint<GetCountryQuery, CountryDto>, IPermissionProvider
{
    public static string Permission => "READ:COUNTRY";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/countries/{id}");
        Description(x => x.WithTags("Country Management")
            .WithSummary("Get country by ID")
            .WithDescription("Retrieves a specific country by its ID. Requires READ:COUNTRY permission."));
    }

    public override async Task HandleAsync(GetCountryQuery req, CancellationToken ct)
    {
        CountryDto country = await Mediator.Send(req, ct);
        await SendSuccessAsync(country, "Country retrieved successfully", ct);
    }
}
