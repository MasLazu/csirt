using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Protocols.Queries.GetProtocol;

public class GetProtocolQuery : IRequest<ProtocolDto>
{
    public Guid Id { get; set; }
}
