namespace MeUi.Application.Interfaces;

public interface ITenantPermissionProvider
{
    static abstract string TenantPermission { get; }
}