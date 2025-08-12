using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantUsers.Queries.GetTenantUserById;

public record GetTenantUserByIdQuery : IRequest<TenantUserDto>
{
    public Guid Id { get; set; }
}