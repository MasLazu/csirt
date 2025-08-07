namespace MeUi.Domain.Entities;

public class RolePermission : BaseEntity
{
    public Guid PermissionId { get; set; }
    public Guid RoleId { get; set; }

    public Permission? Permission { get; set; }
    public Role? Role { get; set; }
}