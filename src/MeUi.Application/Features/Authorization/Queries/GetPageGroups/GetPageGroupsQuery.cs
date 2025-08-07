using MediatR;
using MeUi.Application.Features.Authorization.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetPageGroups;

public class GetPageGroupsQuery : IRequest<IEnumerable<PageGroupDto>>
{
}