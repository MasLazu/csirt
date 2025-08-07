using MediatR;
using MeUi.Application.Features.Authorization.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetAccessiblePages;

public class GetAccessiblePagesQuery : IRequest<IEnumerable<PageGroupDto>>
{
    public Guid UserId { get; set; }
}