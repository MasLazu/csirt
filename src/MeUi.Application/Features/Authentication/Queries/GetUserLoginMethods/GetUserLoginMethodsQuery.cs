using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authentication.Queries.GetUserLoginMethods;

public class GetUserLoginMethodsQuery : IRequest<IEnumerable<LoginMethodDto>>
{
    public Guid UserId { get; set; }
}