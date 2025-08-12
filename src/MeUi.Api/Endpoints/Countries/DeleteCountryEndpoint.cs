using MeUi.Api.Endpoints;
using MeUi.Application.Features.Countries.Commands.DeleteCountry;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Countries;

public class DeleteCountryEndpoint : BaseEndpoint<DeleteCountryCommand, Guid>, IPermissionProvider
{
    public static string Permission => "DELETE:COUNTRY";

    public override void ConfigureEndpoint()
    {
        Delete("api/v1/countries/{id}");
        Description(x => x.WithTags("Country Management")
            .WithSummary("Delete a country")
            .WithDescription("Deletes an existing country. Cannot delete countries referenced by threat events. Requires DELETE:COUNTRY permission."));
    }

    public override async Task HandleAsync(DeleteCountryCommand req, CancellationToken ct)
    {
        Guid countryId = await Mediator.Send(req, ct);
        await SendSuccessAsync(countryId, "Country deleted successfully", ct);
    }
}
