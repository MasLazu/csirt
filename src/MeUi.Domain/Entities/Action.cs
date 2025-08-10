namespace MeUi.Domain.Entities;

public class Action : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<Permission> Permissions { get; set; } = [];
    public ICollection<TenantPermission> TenantPermissions { get; set; } = [];
}