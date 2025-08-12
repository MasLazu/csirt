namespace MeUi.Application.Models;

public class PermissionDto : BaseDto
{
    public Guid ResourceId { get; set; }
    public Guid ActionId { get; set; }
    public string ResourceCode { get; set; } = string.Empty;
    public string ActionCode { get; set; } = string.Empty;

    public ResourceDto? Resource { get; set; }
    public ActionDto? Action { get; set; }
}