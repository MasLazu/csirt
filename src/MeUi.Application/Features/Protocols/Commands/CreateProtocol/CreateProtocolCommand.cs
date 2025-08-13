using MediatR;

namespace MeUi.Application.Features.Protocols.Commands.CreateProtocol;

public class CreateProtocolCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
}
