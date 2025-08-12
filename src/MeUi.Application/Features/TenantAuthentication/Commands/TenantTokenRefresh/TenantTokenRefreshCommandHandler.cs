using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using MeUi.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MeUi.Application.Features.TenantAuthentication.Commands.TenantTokenRefresh;

public class TenantTokenRefreshCommandHandler : IRequestHandler<TenantTokenRefreshCommand, TenantTokenRefreshResponse>
{
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IRepository<TenantUserRefreshToken> _tenantUserRefreshTokenRepository;
    private readonly IRepository<TenantUser> _tenantUserRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public TenantTokenRefreshCommandHandler(
        IRepository<RefreshToken> refreshTokenRepository,
        IRepository<TenantUserRefreshToken> tenantUserRefreshTokenRepository,
        IRepository<TenantUser> tenantUserRepository,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _tenantUserRefreshTokenRepository = tenantUserRefreshTokenRepository;
        _tenantUserRepository = tenantUserRepository;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TenantTokenRefreshResponse> Handle(TenantTokenRefreshCommand request, CancellationToken ct)
    {
        if (!await _jwtTokenService.ValidateTenantRefreshTokenAsync(request.RefreshToken, ct))
        {
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        TenantUser? tenantUser = await _jwtTokenService.GetTenantUserFromRefreshTokenAsync(request.RefreshToken, ct) ??
            throw new UnauthorizedException("Invalid refresh token.");

        if (tenantUser.IsSuspended)
        {
            throw new UnauthorizedException("User account is suspended.");
        }

        tenantUser = await _tenantUserRepository.Query()
            .Include(tu => tu.Tenant)
            .FirstOrDefaultAsync(tu => tu.Id == tenantUser.Id, ct) ??
            throw new UnauthorizedException("User not found.");

        string accessToken = await _jwtTokenService.GenerateTenantAccessTokenAsync(tenantUser, ct);
        string newRefreshToken = await _jwtTokenService.GenerateRefreshTokenAsync(ct);

        RefreshToken? oldRefreshTokenEntity = await _refreshTokenRepository.FirstOrDefaultAsync(
            rt => rt.Token == request.RefreshToken, ct);

        if (oldRefreshTokenEntity != null)
        {
            oldRefreshTokenEntity.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(oldRefreshTokenEntity, ct);
        }

        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var tenantUserRefreshToken = new TenantUserRefreshToken
        {
            TenantUserId = tenantUser.Id,
            RefreshTokenId = newRefreshTokenEntity.Id
        };

        await _tenantUserRefreshTokenRepository.AddAsync(tenantUserRefreshToken, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new TenantTokenRefreshResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
        };
    }
}