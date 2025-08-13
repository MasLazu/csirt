using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;
using System.Linq.Expressions;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventsPaginated;

public class GetThreatEventsPaginatedQueryHandler : IRequestHandler<GetThreatEventsPaginatedQuery, PaginatedDto<ThreatEventDto>>
{
    private readonly IRepository<ThreatEvent> _threatEventRepository;

    public GetThreatEventsPaginatedQueryHandler(IRepository<ThreatEvent> threatEventRepository)
    {
        _threatEventRepository = threatEventRepository;
    }

    public async Task<PaginatedDto<ThreatEventDto>> Handle(GetThreatEventsPaginatedQuery request, CancellationToken ct)
    {
        // Build comprehensive filter predicate
        Expression<Func<ThreatEvent, bool>> predicate = BuildFilterPredicate(request);

        // Build orderBy expression
        Expression<Func<ThreatEvent, object>> orderBy = GetOrderByExpression(request.SortBy);

        // Get paginated threat events using database-level filtering and pagination
        (IEnumerable<ThreatEvent> threatEvents, int totalItems) = await _threatEventRepository.GetPaginatedAsync(
            predicate: predicate,
            orderBy: orderBy,
            orderByDescending: request.IsDescending,
            skip: (request.ValidatedPage - 1) * request.ValidatedPageSize,
            take: request.ValidatedPageSize,
            ct);

        // Convert to DTOs
        List<ThreatEventDto> threatEventDtos = threatEvents.Adapt<List<ThreatEventDto>>();

        return new PaginatedDto<ThreatEventDto>
        {
            Items = threatEventDtos,
            TotalItems = totalItems,
            Page = request.ValidatedPage,
            PageSize = request.ValidatedPageSize,
            TotalPages = (int)Math.Ceiling((double)totalItems / request.ValidatedPageSize)
        };
    }

    private static Expression<Func<ThreatEvent, bool>> BuildFilterPredicate(GetThreatEventsPaginatedQuery request)
    {
        Expression<Func<ThreatEvent, bool>> predicate = threatEvent => true;

        // Time range filtering (CRITICAL for TimescaleDB performance)
        if (request.StartTime.HasValue)
        {
            Expression<Func<ThreatEvent, bool>> startTimeFilter = te => te.Timestamp >= request.StartTime.Value;
            predicate = CombineWithAnd(predicate, startTimeFilter);
        }

        if (request.EndTime.HasValue)
        {
            Expression<Func<ThreatEvent, bool>> endTimeFilter = te => te.Timestamp <= request.EndTime.Value;
            predicate = CombineWithAnd(predicate, endTimeFilter);
        }

        // ASN Registry filtering
        if (request.AsnRegistryId.HasValue)
        {
            Expression<Func<ThreatEvent, bool>> asnFilter = te => te.AsnRegistryId == request.AsnRegistryId.Value;
            predicate = CombineWithAnd(predicate, asnFilter);
        }

        // Location filtering
        if (request.SourceCountryId.HasValue)
        {
            Expression<Func<ThreatEvent, bool>> sourceCountryFilter = te => te.SourceCountryId == request.SourceCountryId.Value;
            predicate = CombineWithAnd(predicate, sourceCountryFilter);
        }

        if (request.DestinationCountryId.HasValue)
        {
            Expression<Func<ThreatEvent, bool>> destCountryFilter = te => te.DestinationCountryId == request.DestinationCountryId.Value;
            predicate = CombineWithAnd(predicate, destCountryFilter);
        }

        // Network filtering
        if (request.SourceAddress != null)
        {
            Expression<Func<ThreatEvent, bool>> sourceAddressFilter = te => te.SourceAddress.Equals(request.SourceAddress);
            predicate = CombineWithAnd(predicate, sourceAddressFilter);
        }

        if (request.DestinationAddress != null)
        {
            Expression<Func<ThreatEvent, bool>> destAddressFilter = te => te.DestinationAddress != null && te.DestinationAddress.Equals(request.DestinationAddress);
            predicate = CombineWithAnd(predicate, destAddressFilter);
        }

        if (request.SourcePort.HasValue)
        {
            Expression<Func<ThreatEvent, bool>> sourcePortFilter = te => te.SourcePort == request.SourcePort.Value;
            predicate = CombineWithAnd(predicate, sourcePortFilter);
        }

        if (request.DestinationPort.HasValue)
        {
            Expression<Func<ThreatEvent, bool>> destPortFilter = te => te.DestinationPort == request.DestinationPort.Value;
            predicate = CombineWithAnd(predicate, destPortFilter);
        }

        if (request.ProtocolId.HasValue)
        {
            Expression<Func<ThreatEvent, bool>> protocolFilter = te => te.ProtocolId == request.ProtocolId.Value;
            predicate = CombineWithAnd(predicate, protocolFilter);
        }

        // Threat classification filtering
        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            string categoryLower = request.Category.ToLower();
            Expression<Func<ThreatEvent, bool>> categoryFilter = te => te.Category.ToLower().Contains(categoryLower);
            predicate = CombineWithAnd(predicate, categoryFilter);
        }

        if (request.MalwareFamilyId.HasValue)
        {
            Expression<Func<ThreatEvent, bool>> malwareFamilyFilter = te => te.MalwareFamilyId == request.MalwareFamilyId.Value;
            predicate = CombineWithAnd(predicate, malwareFamilyFilter);
        }

        // Global search filtering
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            string searchTerm = request.Search.ToLower();
            Expression<Func<ThreatEvent, bool>> searchFilter = te =>
                te.Category.ToLower().Contains(searchTerm) ||
                te.SourceAddress.ToString().Contains(searchTerm) ||
                te.DestinationAddress != null && te.DestinationAddress.ToString().Contains(searchTerm);
            predicate = CombineWithAnd(predicate, searchFilter);
        }

        return predicate;
    }

    private static Expression<Func<ThreatEvent, bool>> CombineWithAnd(
        Expression<Func<ThreatEvent, bool>> left,
        Expression<Func<ThreatEvent, bool>> right)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(ThreatEvent));
        BinaryExpression body = Expression.AndAlso(
            Expression.Invoke(left, parameter),
            Expression.Invoke(right, parameter));
        return Expression.Lambda<Func<ThreatEvent, bool>>(body, parameter);
    }

    private static Expression<Func<ThreatEvent, object>> GetOrderByExpression(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "timestamp" => te => te.Timestamp,
            "sourceaddress" => te => te.SourceAddress,
            "destinationaddress" => te => te.DestinationAddress ?? new System.Net.IPAddress(0),
            "category" => te => te.Category,
            "sourceport" => te => te.SourcePort ?? 0,
            "destinationport" => te => te.DestinationPort ?? 0,
            "createdat" => te => te.CreatedAt,
            _ => te => te.Timestamp // Default to timestamp for time-series data
        };
    }
}
