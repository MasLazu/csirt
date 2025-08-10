using MediatR;
using MeUi.Application.Features.TenantUsers.Models;

namespace MeUi.Application.Features.TenantUsers.Queries.GetTenantUserById;

public record GetTenantUserByIdQuery : IRequest<TenantUserDto>
{
    public Guid Id { get; init; }
}