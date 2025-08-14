using MeUi.Api.Endpoints;
using MeUi.Application.Features.AsnRegistries.Commands.UpdateAsnRegistry;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.AsnRegistries;

public class UpdateAsnRegistryEndpoint : BaseAuthorizedEndpoint<UpdateAsnRegistryCommand, Guid, UpdateAsnRegistryEndpoint>, IPermissionProvider
{
    public static string Permission => "UPDATE:ASN_REGISTRY";

    public override void ConfigureEndpoint()
    {
        Put("api/v1/asn-registries/{id}");
        Description(x => x.WithTags("ASN Registry Management")
            .WithSummary("Update an existing ASN registry")
            .WithDescription("Updates an existing ASN registry with new information. Requires UPDATE:ASN_REGISTRY permission."));
    }
    public override async Task HandleAuthorizedAsync(UpdateAsnRegistryCommand req, Guid userId, CancellationToken ct)
    {
        Guid asnRegistryId = await Mediator.Send(req, ct);
        await SendSuccessAsync(asnRegistryId, "ASN Registry updated successfully", ct);
    }
}
