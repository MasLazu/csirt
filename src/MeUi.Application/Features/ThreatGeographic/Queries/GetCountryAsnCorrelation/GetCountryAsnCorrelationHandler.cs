using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.ThreatGeographic;

namespace MeUi.Application.Features.ThreatGeographic.Queries.GetCountryAsnCorrelation;

public class GetCountryAsnCorrelationHandler : IRequestHandler<GetCountryAsnCorrelationQuery, List<CountryAsnCorrelationDto>>
{
    private readonly IThreatGeographicRepository _repo;

    public GetCountryAsnCorrelationHandler(IThreatGeographicRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<CountryAsnCorrelationDto>> Handle(GetCountryAsnCorrelationQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetCountryAsnCorrelationAsync(request.Start, request.End, request.Limit, cancellationToken);
    }
}
