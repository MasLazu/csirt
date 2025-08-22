using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatGeographic;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatGeographic.Queries.GetCountryAsnCorrelation;

public class GetTenantCountryAsnCorrelationQueryHandler : IRequestHandler<GetTenantCountryAsnCorrelationQuery, List<CountryAsnCorrelationDto>>
{
    private readonly ITenantThreatGeographicRepository _repository;

    public GetTenantCountryAsnCorrelationQueryHandler(ITenantThreatGeographicRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CountryAsnCorrelationDto>> Handle(GetTenantCountryAsnCorrelationQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetCountryAsnCorrelationAsync(request.TenantId, request.Start, request.End, request.Limit, cancellationToken);
    }
}