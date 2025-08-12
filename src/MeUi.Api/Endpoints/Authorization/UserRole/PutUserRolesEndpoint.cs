using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Commands.PutUserRoles;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Authorization.UserRole;

public class PutUserRolesEndpoint : BaseEndpoint<PutUserRolesCommand, IEnumerable<Guid>>, IPermissionProvider
{
    public static string Permission => "UPDATE:USER_ROLE";

    public override void ConfigureEndpoint()
    {
        Put("api/v1/users/{userId}/roles");
        Description(x => x.WithTags("User Authorization").WithSummary("Replace user role assignments"));
    }

    public override async Task HandleAsync(PutUserRolesCommand req, CancellationToken ct)
    {
        IEnumerable<Guid> finalRoles = await Mediator.Send(req, ct);
        await SendSuccessAsync(finalRoles, "User roles updated successfully", ct);
    }
}
