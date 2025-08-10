using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Tenants.Commands.DeleteTenant;

public class DeleteTenantCommandHandler : IRequestHandler<DeleteTenantCommand, Unit>
{
    private readonly IRepository<Tenant> _tenantRepository;
    private readonly IRepository<TenantAsnRegistry> _tenantAsnRegistryRepository;
    private readonly IRepository<TenantUser> _tenantUserRepository;
    private readonly IRepository<TenantRole> _tenantRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTenantCommandHandler(
        IRepository<Tenant> tenantRepository,
        IRepository<TenantAsnRegistry> tenantAsnRegistryRepository,
        IRepository<TenantUser> tenantUserRepository,
        IRepository<TenantRole> tenantRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantRepository = tenantRepository;
        _tenantAsnRegistryRepository = tenantAsnRegistryRepository;
        _tenantUserRepository = tenantUserRepository;
        _tenantRoleRepository = tenantRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteTenantCommand request, CancellationToken ct)
    {
        Tenant tenant = await _tenantRepository.GetByIdAsync(request.Id, ct) ??
            throw new NotFoundException($"Tenant with ID {request.Id} not found");

        IEnumerable<TenantAsnRegistry> tenantAsnRegistries = await _tenantAsnRegistryRepository.FindAsync(
            tar => tar.TenantId == request.Id, ct);

        IEnumerable<TenantUser> tenantUsers = await _tenantUserRepository.FindAsync(
            tu => tu.TenantId == request.Id, ct);

        IEnumerable<TenantRole> tenantRoles = await _tenantRoleRepository.FindAsync(
            tr => tr.TenantId == request.Id, ct);

        await _tenantRepository.DeleteAsync(tenant, ct);
        await _tenantAsnRegistryRepository.DeleteRangeAsync(tenantAsnRegistries, ct);
        await _tenantUserRepository.DeleteRangeAsync(tenantUsers, ct);
        await _tenantRoleRepository.DeleteRangeAsync(tenantRoles, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}