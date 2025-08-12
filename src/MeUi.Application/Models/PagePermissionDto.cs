namespace MeUi.Application.Models;

public class PagePermissionDto : BaseDto
{
    public Guid PageId { get; set; }
    public Guid PermissionId { get; set; }

    public PageDto? Page { get; set; }
    public PermissionDto? Permission { get; set; }
}