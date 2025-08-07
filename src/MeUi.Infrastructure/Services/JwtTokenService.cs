using System.Security.Claims;
using System.Security.Cryptography;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using Microsoft.Extensions.Configuration;
using FastEndpoints.Security;

namespace MeUi.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly IRepository<RefreshToken> _refreshTokenRepository;

    public JwtTokenService(IConfiguration configuration, IRepository<RefreshToken> refreshTokenRepository)
    {
        _configuration = configuration;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public Task<string> GenerateAccessTokenAsync(User user, CancellationToken ct = default)
    {
        var claims = new List<Claim>
        {
            new("sub", user.Id.ToString()),
            new("name", user.Name),
        };

        if (!string.IsNullOrEmpty(user.Email))
        {
            claims.Add(new("email", user.Email));
        }

        if (!string.IsNullOrEmpty(user.Username))
        {
            claims.Add(new("username", user.Username));
        }

        string token = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = GetJwtSecret();
            o.Issuer = GetJwtIssuer();
            o.Audience = GetJwtAudience();
            o.ExpireAt = GetAccessTokenExpiration();
            o.User.Claims.AddRange(claims);
        });

        return Task.FromResult(token);
    }

    public Task<string> GenerateRefreshTokenAsync(CancellationToken ct = default)
    {
        byte[] randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        string tokenValue = Convert.ToBase64String(randomNumber);

        return Task.FromResult(tokenValue);
    }

    public async Task<bool> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var token = await _refreshTokenRepository.FirstOrDefaultAsync(
            t => t.Token == refreshToken &&
                 t.ExpiresAt > DateTime.UtcNow &&
                 t.RevokedAt == null,
            ct);

        return token != null;
    }

    public async Task<User?> GetUserFromRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        // Using Query() to include navigation properties
        var token = _refreshTokenRepository.Query()
            .Where(t => t.Token == refreshToken &&
                       t.ExpiresAt > DateTime.UtcNow &&
                       t.RevokedAt == null)
            .Select(t => t.User)
            .FirstOrDefault();

        return await Task.FromResult(token);
    }

    private DateTime GetAccessTokenExpiration()
    {
        string? expirationMinutesStr = _configuration["Jwt:AccessTokenExpirationMinutes"];
        int expirationMinutes = string.IsNullOrEmpty(expirationMinutesStr) ? 15 : int.Parse(expirationMinutesStr);
        return DateTime.UtcNow.AddMinutes(expirationMinutes);
    }

    private string GetJwtSecret()
    {
        // Try both Key and Secret for backward compatibility
        string? secret = _configuration["Jwt:Key"] ?? _configuration["Jwt:Secret"];
        if (string.IsNullOrEmpty(secret))
        {
            throw new InvalidOperationException("JWT secret is not configured. Please set either Jwt:Key or Jwt:Secret in configuration.");
        }
        return secret;
    }

    private string GetJwtIssuer()
    {
        return _configuration["Jwt:Issuer"] ?? "MeUi.Application";
    }

    private string GetJwtAudience()
    {
        return _configuration["Jwt:Audience"] ?? "MeUi.Application.Users";
    }
}