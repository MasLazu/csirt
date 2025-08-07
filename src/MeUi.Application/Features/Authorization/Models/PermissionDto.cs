namespace MeUi.Application.Features.Authorization.Models;

public record PermissionDto
{
    public Guid Id { get; init; }
    public Guid ResourceId { get; init; }
    public Guid ActionId { get; init; }
    public string ResourceCode { get; init; } = string.Empty;
    public string ActionCode { get; init; } = string.Empty;
    public ResourceDto? Resource { get; init; }
    public ActionDto? Action { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}