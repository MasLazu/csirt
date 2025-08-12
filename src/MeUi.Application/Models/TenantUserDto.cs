namespace MeUi.Application.Models;

public class TenantUserDto : BaseDto
{
    public string? Username { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsSuspended { get; set; }
    public Guid TenantId { get; set; }
}