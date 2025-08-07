namespace MeUi.Domain.Entities;

public class PagePermission : BaseEntity
{
    public Guid PageId { get; set; }
    public Guid PermissionId { get; set; }

    public Page? Page { get; set; }
    public Permission? Permission { get; set; }
}