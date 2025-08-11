using MapsterMapper;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Application.Features.Users.Models;
using MeUi.Domain.Entities;
using System.Linq.Expressions;

namespace MeUi.Application.Features.Users.Queries.GetUsersPaginated;

public class GetUsersPaginatedQueryHandler : IRequestHandler<GetUsersPaginatedQuery, PaginatedResult<UserDto>>
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

    public async Task<PaginatedResult<UserDto>> Handle(GetUsersPaginatedQuery request, CancellationToken ct)
    {
        Expression<Func<User, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            string searchTerm = request.SearchTerm.ToLower();
            var searchPredicate = BuildSearchPredicate(searchTerm);
            predicate = predicate == null ? searchPredicate : CombinePredicates(predicate, searchPredicate);
        }

        if (request.IsSuspended.HasValue)
        {
            var suspendedPredicate = BuildSuspendedPredicate(request.IsSuspended.Value);
            predicate = predicate == null ? suspendedPredicate : CombinePredicates(predicate, suspendedPredicate);
        }

        var result = await _userRepository.GetPaginatedAsync(
            predicate: predicate,
            orderBy: u => u.Name,
            orderByDescending: false,
            skip: (request.PageNumber - 1) * request.PageSize,
            take: request.PageSize,
            ct: ct);

        var userDtos = _mapper.Map<IEnumerable<UserDto>>(result.Items);

        return new PaginatedResult<UserDto>
        {
            Items = userDtos,
            Page = request.PageNumber,
            PageSize = request.PageSize,
            TotalItems = result.TotalCount,
            TotalPages = (int)Math.Ceiling((double)result.TotalCount / request.PageSize)
        };
    }

    private static Expression<Func<User, bool>> BuildSearchPredicate(string searchTerm)
    {
        return tu => (tu.Name != null && tu.Name.ToLower().Contains(searchTerm)) ||
                     (tu.Email != null && tu.Email.ToLower().Contains(searchTerm)) ||
                     (tu.Username != null && tu.Username.ToLower().Contains(searchTerm));
    }

    private static Expression<Func<User, bool>> BuildSuspendedPredicate(bool isSuspended)
    {
        return tu => tu.IsSuspended == isSuspended;
    }

    private static Expression<Func<User, bool>> CombinePredicates(
        Expression<Func<User, bool>> first,
        Expression<Func<User, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(User), "tu");
        var firstBody = ReplaceParameter(first.Body, first.Parameters[0], parameter);
        var secondBody = ReplaceParameter(second.Body, second.Parameters[0], parameter);
        var combined = Expression.AndAlso(firstBody, secondBody);
        return Expression.Lambda<Func<User, bool>>(combined, parameter);
    }

    private static Expression ReplaceParameter(Expression expression, ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        return new ParameterReplacer(oldParameter, newParameter).Visit(expression);
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }
}