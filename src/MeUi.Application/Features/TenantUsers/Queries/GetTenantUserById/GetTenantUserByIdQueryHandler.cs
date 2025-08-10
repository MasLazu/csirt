using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Features.TenantUsers.Models;
using MeUi.Application.Interfaces;

namespace MeUi.Application.Features.TenantUsers.Queries.GetTenantUserById;

public class GetTenantUserByIdQueryHandler : IRequestHandler<GetTenantUserByIdQuery, TenantUserDto>
{
    private readonly ITenantUserRepository _tenantUserRepository;

    public GetTenantUserByIdQueryHandler(ITenantUserRepository tenantUserRepository)
    {
        _tenantUserRepository = tenantUserRepository;
    }

    public async Task<TenantUserDto> Handle(GetTenantUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _tenantUserRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException($"Tenant user with ID {request.Id} not found");
        }

        // Get user roles
        var roles = await _tenantUserRepository.GetUserRolesAsync(user.Id, cancellationToken);

        return new TenantUserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Name = user.Name,
            IsSuspended = user.IsSuspended,
            TenantId = user.TenantId,
            IsTenantAdmin = user.IsTenantAdmin,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Roles = roles.Select(r => r.Name).ToList()
        };
    }
}