namespace MeUi.Domain.Entities;

public class Password : BaseEntity
{
    public Guid UserLoginMethodId { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string? PasswordSalt { get; set; }

    public UserLoginMethod? UserLoginMethod { get; set; }
}