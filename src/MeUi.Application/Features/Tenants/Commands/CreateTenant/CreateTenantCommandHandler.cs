using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Guid>
{
    private readonly IRepository<Tenant> _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTenantCommandHandler(
        IRepository<Tenant> tenantRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateTenantCommand request, CancellationToken ct)
    {
        var tenant = new Tenant
        {
            Name = request.Name,
            Description = request.Description,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone
        };

        await _tenantRepository.AddAsync(tenant, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return tenant.Id;
    }
}