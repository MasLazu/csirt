using MediatR;

namespace MeUi.Application.Features.Tenants.Commands.RemoveAsnFromTenant;

public record RemoveAsnFromTenantCommand : IRequest<Unit>, MeUi.Application.Models.ITenantRequest
{
    public Guid TenantId { get; set; }
    public Guid AsnId { get; set; }
}