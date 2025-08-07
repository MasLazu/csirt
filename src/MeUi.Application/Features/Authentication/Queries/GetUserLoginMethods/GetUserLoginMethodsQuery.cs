using MediatR;
using MeUi.Application.Features.Authentication.Models;

namespace MeUi.Application.Features.Authentication.Queries.GetUserLoginMethods;

public record GetUserLoginMethodsQuery(Guid UserId) : IRequest<IEnumerable<LoginMethodDto>>;