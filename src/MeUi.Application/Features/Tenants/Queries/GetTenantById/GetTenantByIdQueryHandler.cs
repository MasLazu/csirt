using Mapster;
using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Tenants.Queries.GetTenantById;

public sealed class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, TenantDto>
{
    private readonly IRepository<Tenant> _tenantRepository;

    public GetTenantByIdQueryHandler(IRepository<Tenant> tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<TenantDto> Handle(GetTenantByIdQuery request, CancellationToken ct)
    {
        Tenant tenant = await _tenantRepository.GetByIdAsync(request.Id, ct) ??
            throw new NotFoundException($"Tenant with ID {request.Id} not found");
        return tenant.Adapt<TenantDto>();
    }
}
