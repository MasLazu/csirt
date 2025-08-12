using Mapster;
using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetTenantRoleById;

public class GetTenantRoleByIdQueryHandler : IRequestHandler<GetTenantRoleByIdQuery, RoleDto>
{
    private readonly IRepository<TenantRole> _tenantRoleRepository;

    public GetTenantRoleByIdQueryHandler(IRepository<TenantRole> tenantRoleRepository)
    {
        _tenantRoleRepository = tenantRoleRepository;
    }

    public async Task<RoleDto> Handle(GetTenantRoleByIdQuery request, CancellationToken ct)
    {
        // Get the tenant role by ID and ensure it belongs to the correct tenant
        TenantRole? tenantRole = await _tenantRoleRepository.GetByIdAsync(request.Id, ct);

        if (tenantRole == null || tenantRole.TenantId != request.TenantId)
        {
            throw new NotFoundException($"Role with ID {request.Id} not found in tenant {request.TenantId}");
        }

        return tenantRole.Adapt<RoleDto>();
    }
}
