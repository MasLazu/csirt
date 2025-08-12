using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.TenantAuthorization.Commands.CreateTenantRole;

public class CreateTenatRoleCommandHandler : IRequestHandler<CreateTenantRoleCommand, Guid>
{
    private readonly IRepository<TenantRole> _tenantRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTenatRoleCommandHandler(
        IRepository<TenantRole> tenantRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantRoleRepository = tenantRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateTenantRoleCommand request, CancellationToken ct)
    {
        var role = new TenantRole
        {
            Name = request.Name,
            TenantId = request.TenantId,
            Description = request.Description
        };

        await _tenantRoleRepository.AddAsync(role, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return role.Id;
    }
}