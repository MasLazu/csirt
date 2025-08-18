using MapsterMapper;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;
using System.Linq.Expressions;
using MeUi.Application.Utilities;
using Mapster;

namespace MeUi.Application.Features.Users.Queries.GetUsersPaginated;

public class GetUsersPaginatedQueryHandler : IRequestHandler<GetUsersPaginatedQuery, PaginatedDto<UserDto>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<UserRole> _userRoleRepository;

    public GetUsersPaginatedQueryHandler(
        IRepository<User> userRepository,
        IRepository<Role> roleRepository,
        IRepository<UserRole> userRoleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<PaginatedDto<UserDto>> Handle(GetUsersPaginatedQuery request, CancellationToken ct)
    {
        Expression<Func<User, bool>> predicate = u => true;

        if (!string.IsNullOrEmpty(request.Search))
        {
            string search = request.Search;
            Expression<Func<User, bool>> searchPredicate = u =>
                u.Name != null && u.Name.Contains(search) ||
                u.Email != null && u.Email.Contains(search) ||
                u.Username != null && u.Username.Contains(search);
            predicate = predicate.And(searchPredicate);
        }

        if (request.IsSuspended.HasValue)
        {
            bool isSuspended = request.IsSuspended.Value;
            predicate = predicate.And(u => u.IsSuspended == isSuspended);
        }

        Expression<Func<User, object>> orderBy = request.SortBy?.ToLower() switch
        {
            "email" => u => u.Email ?? string.Empty,
            "username" => u => u.Username ?? string.Empty,
            "issuspended" => u => u.IsSuspended,
            "createdat" => u => u.CreatedAt,
            "updatedat" => u => (object)(u.UpdatedAt == null ? DateTime.MinValue : u.UpdatedAt),
            _ => u => (object)(u.Name ?? string.Empty)
        };

        (IEnumerable<User> Items, int TotalCount) result = await _userRepository.GetPaginatedAsync(
            predicate: predicate,
            orderBy: orderBy,
            orderByDescending: request.IsDescending,
            skip: (request.ValidatedPage - 1) * request.ValidatedPageSize,
            take: request.ValidatedPageSize,
            ct: ct);

        var userIds = result.Items.Select(u => u.Id).ToList();

        IEnumerable<UserRole> userRoles = await _userRoleRepository
            .FindAsync(ur => userIds.Contains(ur.UserId), ct);

        IEnumerable<Guid> roleIds = userRoles.Select(ur => ur.RoleId).Distinct();
        IEnumerable<Role> roles = await _roleRepository
            .FindAsync(r => roleIds.Contains(r.Id), ct);

        ILookup<Guid, Guid> userRoleLookup = userRoles.ToLookup(ur => ur.UserId, ur => ur.RoleId);

        IEnumerable<RoleDto> roleDto = roles.Adapt<IEnumerable<RoleDto>>();
        var roleLookup = roleDto.ToDictionary(r => r.Id);

        var userDto = result.Items.Select(user =>
        {
            UserDto dto = user.Adapt<UserDto>();
            IEnumerable<Guid> userRoleIds = userRoleLookup[user.Id];
            dto.Roles = userRoleIds.Where(roleId => roleLookup.ContainsKey(roleId))
                                  .Select(roleId => roleLookup[roleId])
                                  .ToList();
            return dto;
        }).ToList();

        return new PaginatedDto<UserDto>
        {
            Items = userDto,
            Page = request.ValidatedPage,
            PageSize = request.ValidatedPageSize,
            TotalItems = result.TotalCount,
            TotalPages = (int)Math.Ceiling((double)result.TotalCount / request.ValidatedPageSize)
        };
    }
}