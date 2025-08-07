using MapsterMapper;
using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.Username))
        {
            throw new BadRequestException("Either Email or Username must be provided.");
        }

        if (!string.IsNullOrEmpty(request.Email))
        {
            User? existingUserByEmail = await _userRepository.FirstOrDefaultAsync(u => u.Email == request.Email, ct);

            if (existingUserByEmail != null)
            {
                throw new ConflictException($"User with email '{request.Email}' already exists.");
            }
        }

        if (!string.IsNullOrEmpty(request.Username))
        {
            User? existingUserByUsername = await _userRepository.FirstOrDefaultAsync(
                u => u.Username == request.Username, ct);

            if (existingUserByUsername != null)
            {
                throw new ConflictException($"User with username '{request.Username}' already exists.");
            }
        }

        User user = _mapper.Map<User>(request);
        user = await _userRepository.AddAsync(user, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return user.Id;
    }
}