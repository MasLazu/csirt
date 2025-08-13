using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;
using System.Linq.Expressions;

namespace MeUi.Application.Features.Protocols.Queries.GetProtocolsPaginated;

public class GetProtocolsPaginatedQueryHandler : IRequestHandler<GetProtocolsPaginatedQuery, PaginatedDto<ProtocolDto>>
{
    private readonly IRepository<Protocol> _protocolRepository;

    public GetProtocolsPaginatedQueryHandler(IRepository<Protocol> protocolRepository)
    {
        _protocolRepository = protocolRepository;
    }

    public async Task<PaginatedDto<ProtocolDto>> Handle(GetProtocolsPaginatedQuery request, CancellationToken ct)
    {
        Expression<Func<Protocol, bool>> predicate = p => true;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            string term = request.Search.ToLower();
            predicate = p => p.Name.ToLower().Contains(term);
        }

        Expression<Func<Protocol, object>> orderBy = request.SortBy?.ToLowerInvariant() switch
        {
            "name" => p => p.Name,
            "createdat" => p => p.CreatedAt,
            "updatedat" => p => p.UpdatedAt ?? DateTime.MinValue,
            _ => p => p.Name
        };

        (IEnumerable<Protocol> protocols, int total) = await _protocolRepository.GetPaginatedAsync(
            predicate: predicate,
            orderBy: orderBy,
            orderByDescending: request.IsDescending,
            skip: (request.ValidatedPage - 1) * request.ValidatedPageSize,
            take: request.ValidatedPageSize,
            ct: ct);

        IEnumerable<ProtocolDto> items = protocols.Adapt<IEnumerable<ProtocolDto>>();

        return new PaginatedDto<ProtocolDto>
        {
            Items = items,
            TotalItems = total,
            Page = request.ValidatedPage,
            PageSize = request.ValidatedPageSize
        };
    }
}
