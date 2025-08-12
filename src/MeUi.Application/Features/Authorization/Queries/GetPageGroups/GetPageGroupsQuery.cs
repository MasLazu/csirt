using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetPageGroups;

public class GetPageGroupsQuery : IRequest<IEnumerable<PageGroupDto>>;