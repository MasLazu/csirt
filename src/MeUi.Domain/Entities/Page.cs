namespace MeUi.Domain.Entities;

public class Page : BaseEntity
{
    public Guid? ParentId { get; set; }
    public Guid? PageGroupId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;

    public PageGroup? PageGroup { get; set; }
    public ICollection<PagePermission> PagePermissions { get; set; } = new HashSet<PagePermission>();
    public ICollection<PageTenantPermission> PageTenantPermissions { get; set; } = new HashSet<PageTenantPermission>();
}