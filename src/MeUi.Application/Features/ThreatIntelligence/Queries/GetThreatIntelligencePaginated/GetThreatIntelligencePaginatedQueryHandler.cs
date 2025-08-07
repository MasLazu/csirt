using FluentValidation;
using MapsterMapper;
using MediatR;
using MeUi.Application.Features.ThreatIntelligence.Interfaces;
using MeUi.Application.Features.ThreatIntelligence.Models;
using MeUi.Application.Models;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatIntelligencePaginated;

public class GetThreatIntelligencePaginatedQueryHandler : IRequestHandler<GetThreatIntelligencePaginatedQuery, PaginatedResult<ThreatIntelligenceDto>>
{
    private readonly IThreatIntelligenceQueryService _queryService;
    private readonly IMapper _mapper;
    private readonly IValidator<GetThreatIntelligencePaginatedQuery> _validator;

    public GetThreatIntelligencePaginatedQueryHandler(
        IThreatIntelligenceQueryService queryService,
        IMapper mapper,
        IValidator<GetThreatIntelligencePaginatedQuery> validator)
    {
        _queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<PaginatedResult<ThreatIntelligenceDto>> Handle(GetThreatIntelligencePaginatedQuery request, CancellationToken ct)
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
                SortDescending = request.SortDescending,
                Skip = (request.PageNumber - 1) * request.PageSize,
                Take = request.PageSize
            };

            // Get paginated data from service
            var (threatIntelligenceData, totalCount) = await _queryService.GetPaginatedAsync(filter, ct);

            // Map to DTOs
            var threatIntelligenceDtos = _mapper.Map<IEnumerable<ThreatIntelligenceDto>>(threatIntelligenceData);

            // Calculate total pages
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            return new PaginatedResult<ThreatIntelligenceDto>
            {
                Items = threatIntelligenceDtos,
                Page = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalCount,
                TotalPages = totalPages
            };
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"Invalid query parameters: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving paginated threat intelligence data: {ex.Message}", ex);
        }
    }
}