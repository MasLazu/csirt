using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Features.TenantUsers.Models;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantUsers.Queries.GetTenantUsers;

public class GetTenantUsersQueryHandler : IRequestHandler<GetTenantUsersQuery, PaginatedResult<TenantUserDto>>
{
    private readonly ITenantUserRepository _tenantUserRepository;
    private readonly ITenantRepository _tenantRepository;

    public GetTenantUsersQueryHandler(
        ITenantUserRepository tenantUserRepository,
        ITenantRepository tenantRepository)
    {
        _tenantUserRepository = tenantUserRepository;
        _tenantRepository = tenantRepository;
    }

    public async Task<PaginatedResult<TenantUserDto>> Handle(GetTenantUsersQuery request, CancellationToken cancellationToken)
    {
        // Validate tenant exists
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken);
        if (tenant == null)
        {
            throw new NotFoundException($"Tenant with ID {request.TenantId} not found");
        }

        var skip = (request.Page - 1) * request.PageSize;
        var (users, totalCount) = await _tenantUserRepository.GetTenantUsersPaginatedAsync(
            request.TenantId,
            skip,
            request.PageSize,
            request.SearchTerm,
            cancellationToken);

        var userDtos = new List<TenantUserDto>();
        foreach (var user in users)
        {
            // Apply filters if specified
            if (request.IsSuspended.HasValue && user.IsSuspended != request.IsSuspended.Value)
                continue;

            if (request.IsTenantAdmin.HasValue && user.IsTenantAdmin != request.IsTenantAdmin.Value)
                continue;

            // Get user roles
            var roles = await _tenantUserRepository.GetUserRolesAsync(user.Id, cancellationToken);

            userDtos.Add(new TenantUserDto
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
            });
        }

        return new PaginatedResult<TenantUserDto>
        {
            Items = userDtos,
            TotalItems = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }
}