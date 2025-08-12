using System.Text.Json.Serialization;
using MediatR;

namespace MeUi.Application.Commands;

public record BaseTenantCommand
{
    public Guid TenantId { get; set; }
}
