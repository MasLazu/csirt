using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatGeographic;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatGeographic.Queries.GetRegionalTimeZoneActivity;

public class GetTenantRegionalTimeZoneActivityQueryHandler : IRequestHandler<GetTenantRegionalTimeZoneActivityQuery, List<RegionalTimeZoneActivityDto>>
{
    private readonly ITenantThreatGeographicRepository _repository;

    public GetTenantRegionalTimeZoneActivityQueryHandler(ITenantThreatGeographicRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<RegionalTimeZoneActivityDto>> Handle(GetTenantRegionalTimeZoneActivityQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetRegionalTimeZoneActivityAsync(request.TenantId, request.Start, request.End, request.Limit, cancellationToken);
    }
}