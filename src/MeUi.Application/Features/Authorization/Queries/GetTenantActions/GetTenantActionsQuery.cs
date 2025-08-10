using MediatR;
using MeUi.Application.Features.Authorization.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetTenantActions;

public record GetTenantActionsQuery : IRequest<IEnumerable<ActionDto>>;