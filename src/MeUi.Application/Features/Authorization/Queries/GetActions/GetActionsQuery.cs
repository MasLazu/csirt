using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetActions;

public class GetActionsQuery : IRequest<IEnumerable<ActionDto>>;