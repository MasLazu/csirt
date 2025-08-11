using System.Linq.Expressions;
using MediatR;
using MeUi.Application.Features.TenantUsers.Models;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantUsers.Queries.GetTenantUsersPaginated;

public class GetTenantUsersPaginatedQueryHandler : IRequestHandler<GetTenantUsersPaginatedQuery, PaginatedResult<TenantUserDto>>
{
    private readonly IRepository<TenantUser> _tenantUserRepository;

    public GetTenantUsersPaginatedQueryHandler(
        IRepository<TenantUser> tenantUserRepository)
    {
        _tenantUserRepository = tenantUserRepository;
    }

    public async Task<PaginatedResult<TenantUserDto>> Handle(GetTenantUsersPaginatedQuery request, CancellationToken ct)
    {
        Expression<Func<TenantUser, bool>> predicate = tu => tu.TenantId == request.TenantId;

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            string searchTerm = request.SearchTerm.ToLower();
            var searchPredicate = BuildSearchPredicate(searchTerm);
            predicate = CombinePredicates(predicate, searchPredicate);
        }

        if (request.IsSuspended != null)
        {
            var suspendedPredicate = BuildSuspendedPredicate(request.IsSuspended.Value);
            predicate = CombinePredicates(predicate, suspendedPredicate);
        }

        var tenantUsers = await _tenantUserRepository.GetPaginatedAsync(
            predicate,
            orderBy: tu => tu.Name,
            orderByDescending: false,
            skip: (request.Page - 1) * request.PageSize,
            take: request.PageSize,
            ct);

        var tenantUserDtos = tenantUsers.Items.Select(tu => new TenantUserDto
        {
            Id = tu.Id,
            TenantId = tu.TenantId,
            Name = tu.Name,
            Email = tu.Email,
            Username = tu.Username,
            IsSuspended = tu.IsSuspended,
            IsTenantAdmin = tu.TenantUserRoles.Any(tur => tur.TenantRole.Name == "Admin"),
            CreatedAt = tu.CreatedAt,
            UpdatedAt = tu.UpdatedAt,
            Roles = tu.TenantUserRoles.Select(tur => tur.TenantRole.Name).ToList()
        }).ToList();

        return new PaginatedResult<TenantUserDto>
        {
            Items = tenantUserDtos,
            TotalItems = tenantUsers.TotalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)tenantUsers.TotalCount / request.PageSize)
        };
    }

    private static Expression<Func<TenantUser, bool>> BuildSearchPredicate(string searchTerm)
    {
        return tu => (tu.Name != null && tu.Name.ToLower().Contains(searchTerm)) ||
                     (tu.Email != null && tu.Email.ToLower().Contains(searchTerm)) ||
                     (tu.Username != null && tu.Username.ToLower().Contains(searchTerm));
    }

    private static Expression<Func<TenantUser, bool>> BuildSuspendedPredicate(bool isSuspended)
    {
        return tu => tu.IsSuspended == isSuspended;
    }

    private static Expression<Func<TenantUser, bool>> CombinePredicates(
        Expression<Func<TenantUser, bool>> first,
        Expression<Func<TenantUser, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(TenantUser), "tu");
        var firstBody = ReplaceParameter(first.Body, first.Parameters[0], parameter);
        var secondBody = ReplaceParameter(second.Body, second.Parameters[0], parameter);
        var combined = Expression.AndAlso(firstBody, secondBody);
        return Expression.Lambda<Func<TenantUser, bool>>(combined, parameter);
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