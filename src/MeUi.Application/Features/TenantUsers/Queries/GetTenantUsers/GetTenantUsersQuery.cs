using MediatR;
using MeUi.Application.Features.TenantUsers.Models;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantUsers.Queries.GetTenantUsers;

public record GetTenantUsersQuery : IRequest<PaginatedResult<TenantUserDto>>
{
    public Guid TenantId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public bool? IsSuspended { get; init; }
    public bool? IsTenantAdmin { get; init; }
}