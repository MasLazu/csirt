using MeUi.Api.Endpoints;
using MeUi.Application.Features.AsnRegistries.Queries.GetAsnRegistry;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.AsnRegistries;

public class GetAsnRegistryByIdEndpoint : BaseAuthorizedEndpoint<GetAsnRegistryQuery, AsnRegistryDto, GetAsnRegistryByIdEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:ASN_REGISTRY";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/asn-registries/{id}");
        Description(x => x.WithTags("ASN Registry Management")
            .WithSummary("Get ASN registry by ID")
            .WithDescription("Retrieves a specific ASN registry by its unique identifier. Requires READ:ASN_REGISTRY permission."));
    }
    public override async Task HandleAuthorizedAsync(GetAsnRegistryQuery req, Guid userId, CancellationToken ct)
    {
        AsnRegistryDto asnRegistry = await Mediator.Send(req, ct);
        await SendSuccessAsync(asnRegistry, "ASN Registry retrieved successfully", ct);
    }
}
