using MediatR;
using MeUi.Application.Features.Tenants.Models;

namespace MeUi.Application.Features.Tenants.Queries.GetTenantAsnRegistries;

public record GetTenantAsnRegistriesQuery(Guid TenantId) : IRequest<IEnumerable<AsnRegistryDto>>;