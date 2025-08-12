using MeUi.Domain.Entities;

namespace MeUi.Application.Models;

public class UserRoleDto : BaseDto
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public Role? Role { get; set; }
}