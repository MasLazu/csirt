using MediatR;

namespace MeUi.Application.Features.Protocols.Commands.DeleteProtocol;

public class DeleteProtocolCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
}
