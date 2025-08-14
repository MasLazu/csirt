using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Protocols.Queries.GetProtocol;

public class GetProtocolQuery : IRequest<ProtocolDto>, ITenantRequest
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
}
