namespace MeUi.Domain.Entities;

public class User : BaseEntity
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsSuspended { get; set; } = false;

    public ICollection<UserLoginMethod> UserLoginMethods { get; set; } = [];
    public ICollection<UserRefreshToken> UserRefreshTokens { get; set; } = [];
    public ICollection<UserRole> UserRoles { get; set; } = [];
}