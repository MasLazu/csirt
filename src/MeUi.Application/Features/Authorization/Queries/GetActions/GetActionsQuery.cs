using MediatR;
using MeUi.Application.Features.Authorization.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetActions;

public record GetActionsQuery : IRequest<IEnumerable<ActionDto>>;