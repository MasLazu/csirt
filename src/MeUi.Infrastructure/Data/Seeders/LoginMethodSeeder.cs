using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace MeUi.Infrastructure.Data.Seeders;

public class LoginMethodSeeder
{
    private readonly IRepository<LoginMethod> _loginMethodRepository;
    private readonly ILogger<LoginMethodSeeder> _logger;

    public LoginMethodSeeder(IRepository<LoginMethod> loginMethodRepository, ILogger<LoginMethodSeeder> logger)
    {
        _loginMethodRepository = loginMethodRepository;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        var loginMethods = new[]
        {
            new { Code = "PASSWORD", Name = "Password", Description = "Username/Email and Password authentication" },
        };

        foreach (var method in loginMethods)
        {
            LoginMethod? existing = await _loginMethodRepository.FirstOrDefaultAsync(x => x.Code == method.Code, ct);
            if (existing == null)
            {
                await _loginMethodRepository.AddAsync(new LoginMethod
                {
                    Code = method.Code,
                    Name = method.Name,
                    Description = method.Description,
                    IsActive = true
                }, ct);

                _logger.LogInformation("Seeded login method: {Code}", method.Code);
            }
        }
    }
}
