using MediatR;

namespace MeUi.Application.Features.Protocols.Commands.UpdateProtocol;

public class UpdateProtocolCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
