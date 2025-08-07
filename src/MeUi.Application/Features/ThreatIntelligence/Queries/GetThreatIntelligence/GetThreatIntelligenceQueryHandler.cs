using FluentValidation;
using MapsterMapper;
using MediatR;
using MeUi.Application.Features.ThreatIntelligence.Interfaces;
using MeUi.Application.Features.ThreatIntelligence.Models;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatIntelligence;

public class GetThreatIntelligenceQueryHandler : IRequestHandler<GetThreatIntelligenceQuery, IEnumerable<ThreatIntelligenceDto>>
{
    private readonly IThreatIntelligenceQueryService _queryService;
    private readonly IMapper _mapper;
    private readonly IValidator<GetThreatIntelligenceQuery> _validator;

    public GetThreatIntelligenceQueryHandler(
        IThreatIntelligenceQueryService queryService,
        IMapper mapper,
        IValidator<GetThreatIntelligenceQuery> validator)
    {
        _queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<IEnumerable<ThreatIntelligenceDto>> Handle(GetThreatIntelligenceQuery request, CancellationToken ct)
    {
        try
        {
            // Validate the request
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException($"Validation failed: {errors}");
            }

            // Convert query to filter
            var filter = new ThreatIntelligenceFilter
            {
                Asn = request.Asn,
                SourceAddress = request.SourceAddress,
                DestinationAddress = request.DestinationAddress,
                SourceCountry = request.SourceCountry,
                DestinationCountry = request.DestinationCountry,
                Category = request.Category,
                Protocol = request.Protocol,
                SourcePort = request.SourcePort,
                DestinationPort = request.DestinationPort,
                MalwareFamily = request.MalwareFamily,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                SortBy = request.SortBy,
                SortDescending = request.SortDescending
            };

            // Get data from service
            var threatIntelligenceData = await _queryService.GetByFilterAsync(filter, ct);

            // Map to DTOs
            return _mapper.Map<IEnumerable<ThreatIntelligenceDto>>(threatIntelligenceData);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"Invalid query parameters: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving threat intelligence data: {ex.Message}", ex);
        }
    }
}