using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using MeUi.Application.Exceptions;

namespace MeUi.Application.Features.Authorization.Commands.DeleteUserRole;

public class DeleteUserRoleCommandHandler : IRequestHandler<DeleteUserRoleCommand, Guid>
{
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserRoleCommandHandler(
        IRepository<UserRole> userRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(DeleteUserRoleCommand request, CancellationToken ct)
    {
        UserRole userRole = await _userRoleRepository.FirstOrDefaultAsync(
            ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId, ct) ??
            throw new NotFoundException($"UserRole for User '{request.UserId}' and Role '{request.RoleId}' not found");

        await _userRoleRepository.DeleteAsync(userRole, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return userRole.Id;
    }
}
