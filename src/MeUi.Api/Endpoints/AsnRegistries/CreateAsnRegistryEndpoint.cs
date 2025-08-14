using MeUi.Api.Endpoints;
using MeUi.Application.Features.AsnRegistries.Commands.CreateAsnRegistry;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.AsnRegistries;

public class CreateAsnRegistryEndpoint : BaseAuthorizedEndpoint<CreateAsnRegistryCommand, Guid, CreateAsnRegistryEndpoint>, IPermissionProvider
{
    public static string Permission => "CREATE:ASN_REGISTRY";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/asn-registries");
        Description(x => x.WithTags("ASN Registry Management")
            .WithSummary("Create a new ASN registry")
            .WithDescription("Creates a new ASN registry entry. Requires CREATE:ASN_REGISTRY permission."));
    }

    public override async Task HandleAuthorizedAsync(CreateAsnRegistryCommand req, Guid userId, CancellationToken ct)
    {
        Guid asnRegistryId = await Mediator.Send(req, ct);
        await SendSuccessAsync(asnRegistryId, "ASN Registry created successfully", ct);
    }
}
