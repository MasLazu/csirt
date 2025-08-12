using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetTenantPages;

public record GetTenantPagesQuery : IRequest<IEnumerable<PageDto>>;