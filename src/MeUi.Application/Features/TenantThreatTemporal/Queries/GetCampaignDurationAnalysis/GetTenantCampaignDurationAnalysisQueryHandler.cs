using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatTemporal;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Features.TenantThreatTemporal.Queries.GetCampaignDurationAnalysis;

public class GetTenantCampaignDurationAnalysisQueryHandler : IRequestHandler<GetTenantCampaignDurationAnalysisQuery, List<CampaignDurationDto>>
{
    private readonly ITenantThreatTemporalRepository _repository;

    public GetTenantCampaignDurationAnalysisQueryHandler(ITenantThreatTemporalRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CampaignDurationDto>> Handle(GetTenantCampaignDurationAnalysisQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetCampaignDurationAnalysisAsync(
            request.TenantId,
            request.Start,
            request.End,
            request.Limit,
            cancellationToken);
    }
}