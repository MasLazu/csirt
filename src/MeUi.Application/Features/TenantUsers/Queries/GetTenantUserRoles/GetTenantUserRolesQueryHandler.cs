using MediatR;
using Microsoft.EntityFrameworkCore;
using MeUi.Application.Exceptions;
using MeUi.Application.Models;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using Mapster;

namespace MeUi.Application.Features.TenantUsers.Queries.GetTenantUserRoles;

public class GetTenantUserRolesQueryHandler : IRequestHandler<GetTenantUserRolesQuery, IEnumerable<RoleDto>>
{
    private readonly IRepository<TenantUser> _tenantUserRepository;
    private readonly IRepository<TenantUserRole> _tenantUserRoleRepository;

    public GetTenantUserRolesQueryHandler(
        IRepository<TenantUser> tenantUserRepository,
        IRepository<TenantUserRole> tenantUserRoleRepository)
    {
        _tenantUserRepository = tenantUserRepository;
        _tenantUserRoleRepository = tenantUserRoleRepository;
    }

    public async Task<IEnumerable<RoleDto>> Handle(GetTenantUserRolesQuery request, CancellationToken ct)
    {
        // Validate tenant user exists and belongs to the specified tenant
        TenantUser tenantUser = await _tenantUserRepository.FirstOrDefaultAsync(
            tu => tu.Id == request.UserId && tu.TenantId == request.TenantId, ct) ??
            throw new NotFoundException($"Tenant user with ID {request.UserId} not found in tenant {request.TenantId}");

        // Get user roles with role details
        IEnumerable<TenantUserRole> userRoles = await _tenantUserRoleRepository.Query()
            .Include(tur => tur.TenantRole)
            .Where(tur => tur.TenantUserId == request.UserId)
            .ToListAsync(ct);

        return userRoles.Select(tur => new RoleDto
        {
            Id = tur.TenantRole!.Id,
            Name = tur.TenantRole.Name,
            Description = tur.TenantRole.Description
        });
    }
}
