using MediatR;

namespace MeUi.Application.Features.Authorization.Queries.CheckPermission;

public sealed record CheckPermissionQuery(Guid UserId, string PermissionCode) : IRequest<bool>;
