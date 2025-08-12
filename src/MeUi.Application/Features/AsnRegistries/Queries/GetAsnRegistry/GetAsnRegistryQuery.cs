using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.AsnRegistries.Queries.GetAsnRegistry;

public record GetAsnRegistryQuery : IRequest<AsnRegistryDto>
{
    public Guid Id { get; set; }
}
