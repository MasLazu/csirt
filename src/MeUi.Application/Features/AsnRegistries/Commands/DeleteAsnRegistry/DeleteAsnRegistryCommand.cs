using MediatR;

namespace MeUi.Application.Features.AsnRegistries.Commands.DeleteAsnRegistry;

public record DeleteAsnRegistryCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
}
