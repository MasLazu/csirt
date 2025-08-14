using MeUi.Api.Endpoints;
using MeUi.Application.Features.Countries.Commands.CreateCountry;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Countries;

public class CreateCountryEndpoint : BaseAuthorizedEndpoint<CreateCountryCommand, Guid, CreateCountryEndpoint>, IPermissionProvider
{
    public static string Permission => "CREATE:COUNTRY";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/countries");
        Description(x => x.WithTags("Country Management")
            .WithSummary("Create a new country")
            .WithDescription("Creates a new country entry. Requires CREATE:COUNTRY permission."));
    }

    public override async Task HandleAuthorizedAsync(CreateCountryCommand req, Guid userId, CancellationToken ct)
    {
        Guid countryId = await Mediator.Send(req, ct);
        await SendSuccessAsync(countryId, "Country created successfully", ct);
    }
}
