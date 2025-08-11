namespace MeUi.Application.Features.TenantUsers.Models;

public record TenantUserDto
{
    public Guid Id { get; init; }
    public string? Username { get; init; } = string.Empty;
    public string? Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool IsSuspended { get; init; }
    public Guid TenantId { get; init; }
    public bool IsTenantAdmin { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public ICollection<string> Roles { get; init; } = new List<string>();
}