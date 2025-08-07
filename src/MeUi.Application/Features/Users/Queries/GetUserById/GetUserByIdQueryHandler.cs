using MapsterMapper;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Users.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(
        IRepository<User> userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        User? user = await _userRepository.GetByIdAsync(request.Id, ct);

        return user == null ? null : _mapper.Map<UserDto>(user);
    }
}