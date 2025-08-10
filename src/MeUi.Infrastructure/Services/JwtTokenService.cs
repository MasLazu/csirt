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
    private readonly IRepository<UserRefreshToken> _userRefreshTokenRepository;
    private readonly IRepository<TenantUserRefreshToken> _tenantUserRefreshTokenRepository;

    public JwtTokenService(
        IConfiguration configuration,
        IRepository<RefreshToken> refreshTokenRepository,
        IRepository<UserRefreshToken> userRefreshTokenRepository,
        IRepository<TenantUserRefreshToken> tenantUserRefreshTokenRepository)
    {
        _configuration = configuration;
        _refreshTokenRepository = refreshTokenRepository;
        _userRefreshTokenRepository = userRefreshTokenRepository;
        _tenantUserRefreshTokenRepository = tenantUserRefreshTokenRepository;
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
        RefreshToken? token = await _refreshTokenRepository.FirstOrDefaultAsync(
            t => t.Token == refreshToken &&
                 t.ExpiresAt > DateTime.UtcNow &&
                 t.RevokedAt == null,
            ct);

        return token != null;
    }

    public async Task<User?> GetUserFromRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        RefreshToken? refreshTokenEntity = await _refreshTokenRepository.FirstOrDefaultAsync(
            t => t.Token == refreshToken &&
                 t.ExpiresAt > DateTime.UtcNow &&
                 t.RevokedAt == null,
            ct);

        if (refreshTokenEntity == null)
        {
            return null;
        }

        UserRefreshToken? userRefreshToken = await _userRefreshTokenRepository.FirstOrDefaultAsync(
            urt => urt.RefreshTokenId == refreshTokenEntity.Id,
            ct);

        if (userRefreshToken == null)
        {
            return null;
        }

        User? user = _userRefreshTokenRepository.Query()
            .Where(urt => urt.Id == userRefreshToken.Id)
            .Select(urt => urt.User)
            .FirstOrDefault();

        return await Task.FromResult(user);
    }

    public Task<string> GenerateTenantAccessTokenAsync(TenantUser tenantUser, CancellationToken ct = default)
    {
        var claims = new List<Claim>
        {
            new("sub", tenantUser.Id.ToString()),
            new("name", tenantUser.Name),
            new("tenant_id", tenantUser.TenantId.ToString()),
        };

        if (!string.IsNullOrEmpty(tenantUser.Email))
        {
            claims.Add(new("email", tenantUser.Email));
        }

        if (!string.IsNullOrEmpty(tenantUser.Username))
        {
            claims.Add(new("username", tenantUser.Username));
        }

        string token = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = GetJwtSecret();
            o.Issuer = GetJwtIssuer();
            o.Audience = GetTenantJwtAudience();
            o.ExpireAt = GetAccessTokenExpiration();
            o.User.Claims.AddRange(claims);
        });

        return Task.FromResult(token);
    }

    public async Task<bool> ValidateTenantRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        Guid? refreshTokenId = await _refreshTokenRepository.FirstOrDefaultAsync(
            t => t.Token == refreshToken &&
                 t.ExpiresAt > DateTime.UtcNow &&
                 t.RevokedAt == null,
            t => t.Id,
            ct);

        if (refreshTokenId == null)
        {
            return false;
        }

        TenantUserRefreshToken? tenantUserRefreshToken = await _tenantUserRefreshTokenRepository.FirstOrDefaultAsync(
            turt => turt.RefreshTokenId == refreshTokenId,
            ct);

        return tenantUserRefreshToken != null;
    }

    public async Task<TenantUser?> GetTenantUserFromRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        Guid? refreshTokenId = await _refreshTokenRepository.FirstOrDefaultAsync(
            t => t.Token == refreshToken &&
                 t.ExpiresAt > DateTime.UtcNow &&
                 t.RevokedAt == null,
            t => t.Id,
            ct);

        if (refreshTokenId == null)
        {
            return null;
        }

        TenantUserRefreshToken? tenantUserRefreshToken = await _tenantUserRefreshTokenRepository.FirstOrDefaultAsync(
            turt => turt.RefreshTokenId == refreshTokenId,
            ct);

        if (tenantUserRefreshToken == null)
        {
            return null;
        }

        TenantUser? tenantUser = _tenantUserRefreshTokenRepository.Query()
            .Where(turt => turt.Id == tenantUserRefreshToken.Id)
            .Select(turt => turt.TenantUser)
            .FirstOrDefault();

        return await Task.FromResult(tenantUser);
    }

    private DateTime GetAccessTokenExpiration()
    {
        string? expirationMinutesStr = _configuration["Jwt:AccessTokenExpirationMinutes"];
        int expirationMinutes = string.IsNullOrEmpty(expirationMinutesStr) ? 15 : int.Parse(expirationMinutesStr);
        return DateTime.UtcNow.AddMinutes(expirationMinutes);
    }

    private string GetJwtSecret()
    {
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

    private string GetTenantJwtAudience()
    {
        return _configuration["Jwt:TenantAudience"] ?? "MeUi.Application.TenantUsers";
    }
}