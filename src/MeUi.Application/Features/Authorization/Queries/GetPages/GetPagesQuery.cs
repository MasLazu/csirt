using MediatR;
using MeUi.Application.Features.Authorization.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetPages;

public class GetPagesQuery : IRequest<IEnumerable<PageDto>>
{
}