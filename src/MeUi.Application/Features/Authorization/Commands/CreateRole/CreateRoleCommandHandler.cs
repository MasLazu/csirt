using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authorization.Commands.CreateRole;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Guid>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleCommandHandler(
        IRepository<Role> roleRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateRoleCommand request, CancellationToken ct)
    {
        var role = new Role
        {
            Name = request.Name,
            Description = request.Description
        };

        await _roleRepository.AddAsync(role, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return role.Id;
    }
}