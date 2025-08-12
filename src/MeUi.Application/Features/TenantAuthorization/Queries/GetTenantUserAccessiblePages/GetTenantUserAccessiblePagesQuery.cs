using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetTenantUserAccessiblePages;

public class GetTenantUserAccessiblePagesQuery : IRequest<IEnumerable<PageGroupDto>>
{
    public Guid UserId { get; set; }
}