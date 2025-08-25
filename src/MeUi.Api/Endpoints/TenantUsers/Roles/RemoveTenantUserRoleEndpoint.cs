// using MeUi.Api.Endpoints;
// using MeUi.Application.Features.TenantUsers.Commands.RemoveRoleFromTenantUserV2;
// using MeUi.Application.Interfaces;

// namespace MeUi.Api.Endpoints.TenantUsers.Roles;

// public class RemoveTenantUserRoleEndpoint : BaseTenantAuthorizedEndpointWithoutResponse<RemoveRoleFromTenantUserV2Command, RemoveTenantUserRoleEndpoint>, ITenantPermissionProvider, IPermissionProvider
// {
//     public static string TenantPermission => "REMOVE_ROLE:USER";
//     public static string Permission => "REMOVE_ROLE:TENANT_USER";

//     public override void ConfigureEndpoint()
//     {
//         Delete("api/v1/tenants/{TenantId}/users/{UserId}/roles/{RoleId}");
//         Description(x => x.WithTags("Tenant User Roles").WithSummary("Remove a role from a tenant user"));
//     }

//     protected override async Task HandleAuthorizedAsync(RemoveRoleFromTenantUserV2Command req, Guid userId, CancellationToken ct)
//     {
//         await Mediator.Send(req, ct);
//         await SendSuccessAsync("Role removed from tenant user successfully", ct);
//     }
// }
