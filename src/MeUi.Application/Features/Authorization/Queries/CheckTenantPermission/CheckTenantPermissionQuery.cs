using MediatR;

namespace MeUi.Application.Features.Authorization.Queries.CheckTenantPermission;

public sealed record CheckTenantPermissionQuery(Guid TenantUserId, string PermissionCode, Guid TenantId) : IRequest<bool>;
