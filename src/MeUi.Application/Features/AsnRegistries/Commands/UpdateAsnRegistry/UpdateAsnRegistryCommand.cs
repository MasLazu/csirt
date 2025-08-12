using MediatR;

namespace MeUi.Application.Features.AsnRegistries.Commands.UpdateAsnRegistry;

public record UpdateAsnRegistryCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
