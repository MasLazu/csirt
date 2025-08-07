using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Guid>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(UpdateUserCommand request, CancellationToken ct)
    {
        User user = await _userRepository.GetByIdAsync(request.Id, ct) ??
            throw new NotFoundException($"User with ID '{request.Id}' not found.");

        if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.Username))
        {
            throw new BadRequestException("Either Email or Username must be provided.");
        }

        if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
        {
            User? existingUserByEmail = await _userRepository.FirstOrDefaultAsync(
                u => u.Email == request.Email && u.Id != request.Id, ct);

            if (existingUserByEmail != null)
            {
                throw new NotFoundException($"User with email '{request.Email}' already exists.");
            }
        }

        if (!string.IsNullOrEmpty(request.Username) && request.Username != user.Username)
        {
            User? existingUserByUsername = await _userRepository.FirstOrDefaultAsync(
                u => u.Username == request.Username && u.Id != request.Id, ct);

            if (existingUserByUsername != null)
            {
                throw new NotFoundException($"User with username '{request.Username}' already exists.");
            }
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