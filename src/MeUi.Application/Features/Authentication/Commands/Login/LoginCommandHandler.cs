using MapsterMapper;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Authentication.Models;
using MeUi.Domain.Entities;
using RefreshTokenEntity = MeUi.Domain.Entities.RefreshToken;
using Mapster;
using MeUi.Application.Exceptions;

namespace MeUi.Application.Features.Authentication.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenResponse>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<UserLoginMethod> _userLoginMethodRepository;
    private readonly IRepository<Password> _passwordRepository;
    private readonly IRepository<RefreshTokenEntity> _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IRepository<User> userRepository,
        IRepository<UserLoginMethod> userLoginMethodRepository,
        IRepository<Password> passwordRepository,
        IRepository<RefreshTokenEntity> refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _userLoginMethodRepository = userLoginMethodRepository;
        _passwordRepository = passwordRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TokenResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        User? user = await FindUserAsync(request.EmailOrUsername, ct) ??
            throw new UnauthorizedException("Invalid credentials.");

        if (user.IsSuspended)
        {
            throw new UnauthorizedException("User account is suspended.");
        }

        UserLoginMethod userLoginMethod = await _userLoginMethodRepository.FirstOrDefaultAsync(
            ulm => ulm.UserId == user.Id && ulm.LoginMethodCode == "PASSWORD",
            ct) ?? throw new UnauthorizedException("Password authentication not available for this user.");

        Password passwordRecord = await _passwordRepository.FirstOrDefaultAsync(p => p.UserLoginMethodId == userLoginMethod.Id, ct) ??
            throw new UnauthorizedException("Invalid credentials.");

        if (!_passwordHasher.VerifyPassword(request.Password, passwordRecord.PasswordHash))
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        string accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user, ct);
        string refreshToken = await _jwtTokenService.GenerateRefreshTokenAsync(ct);

        var refreshTokenEntity = new RefreshTokenEntity
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        UserInfo userInfo = user.Adapt<UserInfo>();

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = userInfo
        };
    }

    private async Task<User?> FindUserAsync(string emailOrUsername, CancellationToken ct)
    {
        if (emailOrUsername.Contains('@'))
        {
            return await _userRepository.FirstOrDefaultAsync(u => u.Email == emailOrUsername, ct);
        }

        User? userByUsername = await _userRepository.FirstOrDefaultAsync(u => u.Username == emailOrUsername, ct);

        if (userByUsername != null)
        {
            return userByUsername;
        }

        return await _userRepository.FirstOrDefaultAsync(u => u.Email == emailOrUsername, ct);
    }
}