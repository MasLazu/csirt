using MediatR;
using MeUi.Application.Features.Authentication.Models;

namespace MeUi.Application.Features.Authentication.Queries.GetActiveLoginMethods;

public record GetActiveLoginMethodsQuery : IRequest<IEnumerable<LoginMethodDto>>;