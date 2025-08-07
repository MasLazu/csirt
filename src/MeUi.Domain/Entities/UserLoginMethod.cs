namespace MeUi.Domain.Entities;

public class UserLoginMethod : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid LoginMethodId { get; set; }
    public string LoginMethodCode { get; set; } = string.Empty;

    public User? User { get; set; }
    public LoginMethod? LoginMethod { get; set; }
}