namespace MeUi.Domain.Entities;

public class Permission : BaseEntity
{
    public string ResourceCode { get; set; } = string.Empty;
    public string ActionCode { get; set; } = string.Empty;

    public Resource? Resource { get; set; }
    public Action? Action { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
    public ICollection<PagePermission> PagePermissions { get; set; } = [];
}