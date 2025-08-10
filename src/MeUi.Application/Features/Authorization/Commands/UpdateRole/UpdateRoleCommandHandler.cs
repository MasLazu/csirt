using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;


namespace MeUi.Application.Features.Authorization.Commands.UpdateRole;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Guid>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleCommandHandler(
        IRepository<Role> roleRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(UpdateRoleCommand request, CancellationToken ct)
    {
        Role role = await _roleRepository.GetByIdAsync(request.Id, ct) ??
            throw new InvalidOperationException($"Role with ID '{request.Id}' not found");

        Role? existingRole = await _roleRepository.FirstOrDefaultAsync(r => r.Id != request.Id, ct);

        if (existingRole != null)
        {
            throw new InvalidOperationException($"Role with id '{request.Id}' already exists");
        }

        role.Name = request.Name;
        role.Description = request.Description;

        await _roleRepository.UpdateAsync(role, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return role.Id;
    }
}