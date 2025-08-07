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
        Role existingRole = await _roleRepository.FirstOrDefaultAsync(r => r.Code == request.Code, ct) ??
            throw new ConflictException($"Role with code '{request.Code}' already exists.");

        if (existingRole != null)
        {
            throw new ConflictException($"Role with code '{request.Code}' already exists.");
        }

        var role = new Role
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description
        };

        await _roleRepository.AddAsync(role, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return role.Id;
    }
}