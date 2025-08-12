using MeUi.Api.Endpoints;
using MeUi.Application.Features.AsnRegistries.Commands.DeleteAsnRegistry;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.AsnRegistries;

public class DeleteAsnRegistryEndpoint : BaseEndpoint<DeleteAsnRegistryCommand, Guid>, IPermissionProvider
{
    public static string Permission => "DELETE:ASN_REGISTRY";

    public override void ConfigureEndpoint()
    {
        Delete("api/v1/asn-registries/{id}");
        Description(x => x.WithTags("ASN Registry Management")
            .WithSummary("Delete an ASN registry")
            .WithDescription("Deletes an ASN registry. Cannot delete if it has tenant assignments or associated threat events. Requires DELETE:ASN_REGISTRY permission."));
    }

    public override async Task HandleAsync(DeleteAsnRegistryCommand req, CancellationToken ct)
    {
        Guid asnRegistryId = await Mediator.Send(req, ct);
        await SendSuccessAsync(asnRegistryId, "ASN Registry deleted successfully", ct);
    }
}
