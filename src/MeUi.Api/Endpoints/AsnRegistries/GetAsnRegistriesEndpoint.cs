using MeUi.Api.Endpoints;
using MeUi.Application.Features.AsnRegistries.Queries.GetAsnRegistriesPaginated;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.AsnRegistries;

public class GetAsnRegistriesEndpoint : BaseAuthorizedEndpoint<GetAsnRegistriesPaginatedQuery, PaginatedDto<AsnRegistryDto>, GetAsnRegistriesEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:ASN_REGISTRY";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/asn-registries");
        Description(x => x.WithTags("ASN Registry Management")
            .WithSummary("Get paginated list of ASN registries")
            .WithDescription("Retrieves a paginated list of all ASN registries with optional search and sorting. Requires READ:ASN_REGISTRY permission."));
    }

    public override async Task HandleAuthorizedAsync(GetAsnRegistriesPaginatedQuery req, Guid userId, CancellationToken ct)
    {
        PaginatedDto<AsnRegistryDto> asnRegistries = await Mediator.Send(req, ct);
        await SendSuccessAsync(asnRegistries, "ASN Registries retrieved successfully", ct);
    }
}
