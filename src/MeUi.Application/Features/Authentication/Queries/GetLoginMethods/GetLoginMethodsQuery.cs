using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authentication.Queries.GetLoginMethods;

public record GetLoginMethodsQuery : IRequest<IEnumerable<LoginMethodDto>>;