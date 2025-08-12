using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthentication.Queries.GetTenantUserLoginMethods;

public class GetTenantUserLoginMethodsQuery : IRequest<IEnumerable<LoginMethodDto>>
{
    public Guid UserId { get; set; }
};