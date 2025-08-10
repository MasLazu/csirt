using MapsterMapper;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Authentication.Models;
using MeUi.Domain.Entities;
using RefreshTokenEntity = MeUi.Domain.Entities.RefreshToken;
using MeUi.Application.Exceptions;
using Mapster;

namespace MeUi.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponse>
{
    private readonly IRepository<RefreshTokenEntity> _refreshTokenRepository;
    private readonly IRepository<UserRefreshToken> _userRefreshTokenRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IRepository<RefreshTokenEntity> refreshTokenRepository,
        IRepository<UserRefreshToken> userRefreshTokenRepository,
        IRepository<User> userRepository,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRefreshTokenRepository = userRefreshTokenRepository;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TokenResponse> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        RefreshTokenEntity refreshTokenEntity = await _refreshTokenRepository.FirstOrDefaultAsync(
            rt => rt.Token == request.RefreshToken, ct) ??
            throw new UnauthorizedException("Invalid refresh token.");

        if (refreshTokenEntity.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Refresh token has expired.");
        }

        if (refreshTokenEntity.RevokedAt.HasValue)
        {
            throw new UnauthorizedException("Refresh token has been revoked.");
        }

        // Find the user through the pivot table
        UserRefreshToken userRefreshToken = await _userRefreshTokenRepository.FirstOrDefaultAsync(
            urt => urt.RefreshTokenId == refreshTokenEntity.Id, ct) ??
            throw new UnauthorizedException("Invalid refresh token association.");

        User user = await _userRepository.GetByIdAsync(userRefreshToken.UserId, ct) ??
            throw new UnauthorizedException("User not found.");

        if (user.IsSuspended)
        {
            throw new ForbiddenException("User account is suspended.");
        }

        string accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user, ct);
        string newRefreshToken = await _jwtTokenService.GenerateRefreshTokenAsync(ct);

        refreshTokenEntity.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(refreshTokenEntity, ct);

        var newRefreshTokenEntity = new RefreshTokenEntity
        {
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var newUserRefreshToken = new UserRefreshToken
        {
            UserId = user.Id,
            RefreshTokenId = newRefreshTokenEntity.Id
        };

        await _userRefreshTokenRepository.AddAsync(newUserRefreshToken, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        UserInfo userInfo = user.Adapt<UserInfo>();

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = userInfo
        };
    }
}