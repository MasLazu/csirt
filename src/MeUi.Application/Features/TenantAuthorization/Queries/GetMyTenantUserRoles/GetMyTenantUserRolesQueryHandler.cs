using Mapster;
using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetMyTenantUserRoles;

public class GetMyTenantUserRolesQueryHandler : IRequestHandler<GetMyTenantUserRolesQuery, IEnumerable<TenantRoleDto>>
{
    private readonly IRepository<TenantUserRole> _tenantUserRoleRepository;
    private readonly IRepository<TenantRole> _tenantRoleRepository;
    private readonly IRepository<TenantUser> _tenantUserRepository;

    public GetMyTenantUserRolesQueryHandler(
        IRepository<TenantUserRole> tenantUserRoleRepository,
        IRepository<TenantRole> tenantRoleRepository,
        IRepository<TenantUser> tenantUserRepository)
    {
        _tenantUserRoleRepository = tenantUserRoleRepository;
        _tenantRoleRepository = tenantRoleRepository;
        _tenantUserRepository = tenantUserRepository;
    }

    public async Task<IEnumerable<TenantRoleDto>> Handle(GetMyTenantUserRolesQuery request, CancellationToken ct)
    {
        TenantUser? tenantUser = await _tenantUserRepository.GetByIdAsync(request.TenantUserId, ct) ??
            throw new NotFoundException($"Tenant user with ID {request.TenantUserId} not found");

        IEnumerable<Guid> roleIds = await _tenantUserRoleRepository.FindAsync(
            tur => tur.TenantUserId == request.TenantUserId, tur => tur.TenantRoleId, ct);

        IEnumerable<TenantRole> roles = await _tenantRoleRepository
            .FindAsync(tr => roleIds.Contains(tr.Id) && tr.TenantId == tenantUser.TenantId, ct);

        return roles.Adapt<IEnumerable<TenantRoleDto>>();
    }
}
