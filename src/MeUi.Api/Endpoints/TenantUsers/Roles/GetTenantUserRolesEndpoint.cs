// using MeUi.Api.Endpoints;
// using MeUi.Application.Features.TenantUsers.Queries.GetTenantUserRoles;
// using MeUi.Application.Models;
// using MeUi.Application.Interfaces;

// namespace MeUi.Api.Endpoints.TenantUsers.Roles;

// public class GetTenantUserRolesEndpoint : BaseTenantAuthorizedEndpoint<GetTenantUserRolesQuery, IEnumerable<RoleDto>, GetTenantUserRolesEndpoint>, ITenantPermissionProvider, IPermissionProvider
// {
//     public static string TenantPermission => "READ:USER_ROLES";
//     public static string Permission => "READ:TENANT_USER_ROLES";

//     public override void ConfigureEndpoint()
//     {
//         Get("api/v1/tenants/{TenantId}/users/{UserId}/roles");
//         Description(x => x.WithTags("Tenant User Roles").WithSummary("Get roles assigned to a tenant user"));
//     }

//     protected override async Task HandleAuthorizedAsync(GetTenantUserRolesQuery req, Guid userId, CancellationToken ct)
//     {
//         IEnumerable<RoleDto> result = await Mediator.Send(req, ct);
//         await SendSuccessAsync(result, "Tenant user roles retrieved successfully", ct);
//     }
// }
