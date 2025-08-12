using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetPages;

public class GetPagesQuery : IRequest<IEnumerable<PageDto>>;