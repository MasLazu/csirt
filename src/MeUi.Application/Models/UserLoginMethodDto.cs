namespace MeUi.Application.Models;

public class UserLoginMethodDto : BaseDto
{
    public Guid UserId { get; set; }
    public string LoginMethodCode { get; set; } = string.Empty;

    public LoginMethodDto? LoginMethod { get; set; }
}