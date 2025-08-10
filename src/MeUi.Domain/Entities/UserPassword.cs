namespace MeUi.Domain.Entities;

public class UserPassword : BaseEntity
{
    public Guid PasswordId { get; set; }
    public Guid UserLoginMethodId { get; set; }

    public Password? Password { get; set; }
    public UserLoginMethod? UserLoginMethod { get; set; }
}