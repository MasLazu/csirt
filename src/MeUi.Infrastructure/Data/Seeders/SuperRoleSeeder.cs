using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace MeUi.Infrastructure.Data.Seeders;

public class SuperRoleSeeder
{
    private readonly IRepository<Role> _roleRepository;
    private readonly ILogger<SuperRoleSeeder> _logger;

    public SuperRoleSeeder(IRepository<Role> roleRepository, ILogger<SuperRoleSeeder> logger)
    {
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        var role = new Role
        {
            Id = Guid.Parse("01989299-2c61-71a0-92b9-5a7700dd263e"),
            Name = "Super Administrator",
            Description = "Full system access"
        };

        Role? existing = await _roleRepository.FirstOrDefaultAsync(x => x.Id == role.Id, ct);
        if (existing == null)
        {
            await _roleRepository.AddAsync(role, ct);
            _logger.LogInformation("Seeded role: {Code}", role.Name);
        }
    }
}
