using MapsterMapper;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using Mapster;
using MeUi.Application.Exceptions;

namespace MeUi.Application.Features.Authentication.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<UserLoginMethod> _userLoginMethodRepository;
    private readonly IRepository<UserPassword> _userPasswordRepository;
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IRepository<UserRefreshToken> _userRefreshTokenRepository;
    private readonly IRepository<Password> _passwordRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IRepository<User> userRepository,
        IRepository<UserLoginMethod> userLoginMethodRepository,
        IRepository<UserPassword> userPasswordRepository,
        IRepository<RefreshToken> refreshTokenRepository,
        IRepository<UserRefreshToken> userRefreshTokenRepository,
        IRepository<Password> passwordRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _userLoginMethodRepository = userLoginMethodRepository;
        _userPasswordRepository = userPasswordRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _userRefreshTokenRepository = userRefreshTokenRepository;
        _passwordRepository = passwordRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        User? user = await FindUserAsync(request.Identifier, ct) ??
            throw new UnauthorizedException("Invalid credentials.");

        if (user.IsSuspended)
        {
            throw new UnauthorizedException("User account is suspended.");
        }

        Guid? userLoginMethodId = await _userLoginMethodRepository.FirstOrDefaultAsync(
            ulm => ulm.UserId == user.Id && ulm.LoginMethodCode == "PASSWORD",
            ulm => ulm.Id,
            ct);

        if (userLoginMethodId == null)
        {
            throw new UnauthorizedException("Password authentication not available for this user.");
        }

        Guid? userPasswordId = await _userPasswordRepository.FirstOrDefaultAsync(
            up => up.UserLoginMethodId == userLoginMethodId,
            up => up.PasswordId,
            ct);

        if (userPasswordId == null)
        {
            throw new UnauthorizedException("Password not set for this user.");
        }

        Password? userPassword = await _passwordRepository.GetByIdAsync(userPasswordId.Value, ct) ??
            throw new UnauthorizedException("Password not found for this user.");

        if (!_passwordHasher.VerifyPassword(request.Password, userPassword.PasswordHash))
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        string accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user, ct);
        string refreshToken = await _jwtTokenService.GenerateRefreshTokenAsync(ct);

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var userRefreshToken = new UserRefreshToken
        {
            UserId = user.Id,
            RefreshTokenId = refreshTokenEntity.Id
        };

        await _userRefreshTokenRepository.AddAsync(userRefreshToken, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        UserInfo userInfo = user.Adapt<UserInfo>();

        return new LoginResponse
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