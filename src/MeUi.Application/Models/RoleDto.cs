namespace MeUi.Application.Models;

public class RoleDto : BaseDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public IEnumerable<PermissionDto> Permissions { get; set; } = [];
}