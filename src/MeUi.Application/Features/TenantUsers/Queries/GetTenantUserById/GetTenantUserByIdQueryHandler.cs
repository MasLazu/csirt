using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Features.TenantUsers.Models;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantUsers.Queries.GetTenantUserById;

public class GetTenantUserByIdQueryHandler : IRequestHandler<GetTenantUserByIdQuery, TenantUserDto>
{
    private readonly IRepository<TenantUser> _tenantUserRepository;

    public GetTenantUserByIdQueryHandler(
        IRepository<TenantUser> tenantUserRepository)
    {
        _tenantUserRepository = tenantUserRepository;
    }

    public async Task<TenantUserDto> Handle(GetTenantUserByIdQuery request, CancellationToken cancellationToken)
    {
        TenantUser tenantUser = await _tenantUserRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Tenant user not found.");

        return new TenantUserDto
        {
            Id = tenantUser.Id,
            Email = tenantUser.Email,
            Username = tenantUser.Username,
            Name = tenantUser.Name,
            TenantId = tenantUser.TenantId
        };
    }
}