using MediatR;
using MeUi.Application.Features.Authorization.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetTenantUserAccessiblePages;

public record GetTenantUserAccessiblePagesQuery(Guid UserId) : IRequest<IEnumerable<PageDto>>;