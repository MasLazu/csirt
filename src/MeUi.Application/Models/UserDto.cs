namespace MeUi.Application.Models;

public class UserDto : BaseDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsSuspended { get; set; }
    public IEnumerable<RoleDto> Roles { get; set; } = [];
}