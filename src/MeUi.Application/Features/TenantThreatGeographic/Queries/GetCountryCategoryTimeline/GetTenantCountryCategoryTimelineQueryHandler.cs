using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatGeographic;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatGeographic.Queries.GetCountryCategoryTimeline;

public class GetTenantCountryCategoryTimelineQueryHandler : IRequestHandler<GetTenantCountryCategoryTimelineQuery, List<CountryCategoryTimelineDto>>
{
    private readonly ITenantThreatGeographicRepository _repository;

    public GetTenantCountryCategoryTimelineQueryHandler(ITenantThreatGeographicRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CountryCategoryTimelineDto>> Handle(GetTenantCountryCategoryTimelineQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetCountryCategoryTimelineAsync(request.TenantId, request.Start, request.End, request.Interval, request.TopCountries, cancellationToken);
    }
}