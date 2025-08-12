using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authentication.Queries.GetActiveLoginMethods;

public class GetActiveLoginMethodsQuery : IRequest<IEnumerable<LoginMethodDto>>;