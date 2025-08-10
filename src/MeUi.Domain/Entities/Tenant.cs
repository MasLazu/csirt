namespace MeUi.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<TenantAsnRegistry> TenantAsnRegistries { get; set; } = [];
    public ICollection<TenantUser> TenantUsers { get; set; } = [];
    public ICollection<TenantRole> TenantRoles { get; set; } = [];
}