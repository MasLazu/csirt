using MapsterMapper;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;
using System.Linq.Expressions;

namespace MeUi.Application.Features.Users.Queries.GetUsersPaginated;

public class GetUsersPaginatedQueryHandler : IRequestHandler<GetUsersPaginatedQuery, PaginatedDto<UserDto>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;

    public GetUsersPaginatedQueryHandler(
        IRepository<User> userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedDto<UserDto>> Handle(GetUsersPaginatedQuery request, CancellationToken ct)
    {
        // Build the predicate for filtering
        Expression<Func<User, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(request.Search))
        {
            predicate = u => u.Name != null && u.Name.Contains(request.Search) ||
                            u.Email != null && u.Email.Contains(request.Search) ||
                            u.Username != null && u.Username.Contains(request.Search);
        }

        if (request.IsSuspended.HasValue)
        {
            bool isSuspended = request.IsSuspended.Value;
            predicate = predicate == null
                ? u => u.IsSuspended == isSuspended
                : u => u.IsSuspended == isSuspended &&
                       (u.Name != null && u.Name.Contains(request.Search!) ||
                        u.Email != null && u.Email.Contains(request.Search!) ||
                        u.Username != null && u.Username.Contains(request.Search!));
        }

        // Determine sort field and direction
        Expression<Func<User, object>> orderBy = request.SortBy?.ToLower() switch
        {
            "email" => u => u.Email ?? string.Empty,
            "username" => u => u.Username ?? string.Empty,
            "issuspended" => u => u.IsSuspended,
            "createdat" => u => u.CreatedAt,
            "updatedat" => u => u.UpdatedAt,
            _ => u => u.Name ?? string.Empty // Default sort by name
        };

        (IEnumerable<User> Items, int TotalCount) result = await _userRepository.GetPaginatedAsync(
            predicate: predicate,
            orderBy: orderBy,
            orderByDescending: request.IsDescending,
            skip: (request.ValidatedPage - 1) * request.ValidatedPageSize,
            take: request.ValidatedPageSize,
            ct: ct);

        IEnumerable<UserDto> userDtos = _mapper.Map<IEnumerable<UserDto>>(result.Items);

        return new PaginatedDto<UserDto>
        {
            Items = userDtos,
            Page = request.ValidatedPage,
            PageSize = request.ValidatedPageSize,
            TotalItems = result.TotalCount,
            TotalPages = (int)Math.Ceiling((double)result.TotalCount / request.ValidatedPageSize)
        };
    }
}