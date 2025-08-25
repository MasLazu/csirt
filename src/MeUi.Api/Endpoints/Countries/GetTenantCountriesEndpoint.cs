using MeUi.Api.Endpoints;
using MeUi.Application.Features.Countries.Queries.GetCountriesPaginated;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Api.Models;

namespace MeUi.Api.Endpoints.Countries;

public class GetTenantCountriesEndpoint : BaseTenantAuthorizedEndpoint<GetTenantCountriesRequest, PaginatedDto<CountryDto>, GetTenantCountriesEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:COUNTRY";
    public static string Permission => "READ:TENANT_COUNTRY";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/countries");
        Description(x => x.WithTags("Tenant Country").WithSummary("Get paginated list of countries for a tenant"));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantCountriesRequest req, Guid userId, CancellationToken ct)
    {
        var query = new GetCountriesPaginatedQuery
        {
            Page = req.Page,
            PageSize = req.PageSize,
            Search = req.Search,
            SortBy = req.SortBy,
            SortDirection = req.SortDirection
        };

        PaginatedDto<CountryDto> countries = await Mediator.Send(query, ct);
        await SendSuccessAsync(countries, $"Retrieved {countries.Items.Count()} countries successfully", ct);
    }
}
