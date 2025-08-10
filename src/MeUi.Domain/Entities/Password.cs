namespace MeUi.Domain.Entities;

public class Password : BaseEntity
{
    public string PasswordHash { get; set; } = string.Empty;
    public string? PasswordSalt { get; set; }

    public ICollection<UserPassword> UserPasswords { get; set; } = [];
    public ICollection<TenantUserPassword> TenantUserPasswords { get; set; } = [];
}