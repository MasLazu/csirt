using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Guid>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<TenantUser> _tenantUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IRepository<User> userRepository,
        IRepository<TenantUser> tenantUserRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tenantUserRepository = tenantUserRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(UpdateUserCommand request, CancellationToken ct)
    {
        User user = await _userRepository.GetByIdAsync(request.Id, ct) ??
            throw new NotFoundException($"User with ID '{request.Id}' not found.");

        if (await _userRepository.ExistsAsync(u => (u.Email == request.Email || u.Username == request.Username) && u.Id != request.Id, ct))
        {
            throw new BadRequestException("User with this email already exists.");
        }

        if (await _tenantUserRepository.ExistsAsync(tu => tu.Email == request.Email || tu.Username == request.Username, ct))
        {
            throw new BadRequestException("Tenant user with this email already exists.");
        }

        user.Username = request.Username;
        user.Email = request.Email;
        user.Name = request.Name;
        user.IsSuspended = request.IsSuspended;

        await _userRepository.UpdateAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return user.Id;
    }
}