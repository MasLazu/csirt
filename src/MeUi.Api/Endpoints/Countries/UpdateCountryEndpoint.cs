using MeUi.Api.Endpoints;
using MeUi.Application.Features.Countries.Commands.UpdateCountry;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Countries;

public class UpdateCountryEndpoint : BaseAuthorizedEndpoint<UpdateCountryCommand, Guid, UpdateCountryEndpoint>, IPermissionProvider
{
    public static string Permission => "UPDATE:COUNTRY";

    public override void ConfigureEndpoint()
    {
        Put("api/v1/countries/{id}");
        Description(x => x.WithTags("Country Management")
            .WithSummary("Update an existing country")
            .WithDescription("Updates an existing country's information. Requires UPDATE:COUNTRY permission."));
    }

    public override async Task HandleAuthorizedAsync(UpdateCountryCommand req, Guid userId, CancellationToken ct)
    {
        Guid countryId = await Mediator.Send(req, ct);
        await SendSuccessAsync(countryId, "Country updated successfully", ct);
    }
}
