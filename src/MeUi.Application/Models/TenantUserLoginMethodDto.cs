namespace MeUi.Application.Models;

public class TenantUserLoginMethod : BaseDto
{
    public Guid TenantUserId { get; set; }
    public string LoginMethodCode { get; set; } = string.Empty;

    public LoginMethodDto? LoginMethod { get; set; }
}