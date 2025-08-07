using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;


namespace MeUi.Application.Features.Authorization.Commands.DeleteRole;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Guid>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoleCommandHandler(
        IRepository<Role> roleRepository,
        IRepository<UserRole> userRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(DeleteRoleCommand request, CancellationToken ct)
    {
        Role role = await _roleRepository.GetByIdAsync(request.Id, ct) ??
            throw new InvalidOperationException($"Role with ID '{request.Id}' not found");

        UserRole? userRole = await _userRoleRepository.FirstOrDefaultAsync(ur => ur.RoleId == request.Id, ct) ??
            throw new InvalidOperationException("Cannot delete role that is assigned to users");

        await _roleRepository.DeleteAsync(role, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return role.Id;
    }
}