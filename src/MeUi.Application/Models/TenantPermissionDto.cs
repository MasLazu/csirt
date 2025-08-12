namespace MeUi.Application.Models;

public class TenantPermissionDto : BaseDto
{
    public string ResourceCode { get; set; } = string.Empty;
    public string ActionCode { get; set; } = string.Empty;

    public ResourceDto? Resource { get; set; }
    public ActionDto? Action { get; set; }
}