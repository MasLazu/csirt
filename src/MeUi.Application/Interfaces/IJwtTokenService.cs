using MeUi.Domain.Entities;

namespace MeUi.Application.Interfaces;

public interface IJwtTokenService
{
    Task<string> GenerateAccessTokenAsync(User user, CancellationToken ct = default);
    Task<string> GenerateRefreshTokenAsync(CancellationToken ct = default);
    Task<bool> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<User?> GetUserFromRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}