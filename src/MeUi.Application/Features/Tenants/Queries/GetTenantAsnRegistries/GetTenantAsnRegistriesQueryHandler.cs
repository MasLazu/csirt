using Mapster;
using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Tenants.Queries.GetTenantAsnRegistries;

public class GetTenantAsnRegistriesQueryHandler : IRequestHandler<GetTenantAsnRegistriesQuery, IEnumerable<AsnRegistryDto>>
{
    private readonly IRepository<TenantAsnRegistry> _tenantAsnRegistryRepository;
    private readonly IRepository<AsnRegistry> _asnRegistryRepository;

    public GetTenantAsnRegistriesQueryHandler(
        IRepository<TenantAsnRegistry> tenantAsnRegistryRepository,
        IRepository<AsnRegistry> asnRegistryRepository)
    {
        _tenantAsnRegistryRepository = tenantAsnRegistryRepository;
        _asnRegistryRepository = asnRegistryRepository;
    }

    public async Task<IEnumerable<AsnRegistryDto>> Handle(GetTenantAsnRegistriesQuery request, CancellationToken ct)
    {
        IEnumerable<TenantAsnRegistry> tenantAsnRegistries = await _tenantAsnRegistryRepository
            .FindAsync(tar => tar.TenantId == request.TenantId, ct);

        IEnumerable<AsnRegistry> asnRegistries = await _asnRegistryRepository.GetAllAsync(ct);

        return asnRegistries.Adapt<IEnumerable<AsnRegistryDto>>();
    }
}