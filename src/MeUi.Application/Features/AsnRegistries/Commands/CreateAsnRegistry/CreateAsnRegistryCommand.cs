using MediatR;

namespace MeUi.Application.Features.AsnRegistries.Commands.CreateAsnRegistry;

public record CreateAsnRegistryCommand : IRequest<Guid>
{
    public string Number { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
