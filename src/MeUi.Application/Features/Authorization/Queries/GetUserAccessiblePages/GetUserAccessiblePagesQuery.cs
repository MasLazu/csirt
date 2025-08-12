using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetUserAccessiblePages;

public class GetUserAccessiblePagesQuery : IRequest<IEnumerable<PageGroupDto>>
{
    public Guid UserId { get; set; }
}
