using MeUi.Application.Models;

namespace MeUi.Api.Models;

public class GetTenantUserMeRequest : ITenantRequest
{
    public Guid TenantId { get; set; }
}
