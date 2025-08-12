using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using Mapster;
using MeUi.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MeUi.Application.Features.TenantAuthentication.Commands.TenantLogin;

public class TenantLoginCommandHandler : IRequestHandler<TenantLoginCommand, TenantLoginResponse>
{
    private readonly IRepository<TenantUser> _tenantUserRepository;
    private readonly IRepository<TenantUserLoginMethod> _tenantUserLoginMethodRepository;
    private readonly IRepository<TenantUserPassword> _tenantUserPasswordRepository;
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IRepository<TenantUserRefreshToken> _tenantUserRefreshTokenRepository;
    private readonly IRepository<Password> _passwordRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public TenantLoginCommandHandler(
        IRepository<TenantUser> tenantUserRepository,
        IRepository<TenantUserLoginMethod> tenantUserLoginMethodRepository,
        IRepository<TenantUserPassword> tenantUserPasswordRepository,
        IRepository<RefreshToken> refreshTokenRepository,
        IRepository<TenantUserRefreshToken> tenantUserRefreshTokenRepository,
        IRepository<Password> passwordRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _tenantUserRepository = tenantUserRepository;
        _tenantUserLoginMethodRepository = tenantUserLoginMethodRepository;
        _tenantUserPasswordRepository = tenantUserPasswordRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tenantUserRefreshTokenRepository = tenantUserRefreshTokenRepository;
        _passwordRepository = passwordRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TenantLoginResponse> Handle(TenantLoginCommand request, CancellationToken ct)
    {
        TenantUser? tenantUser = await FindTenantUserAsync(request.Identifier, request.TenantId, ct) ??
            throw new UnauthorizedException("Invalid credentials.");

        if (tenantUser.IsSuspended)
        {
            throw new UnauthorizedException("User account is suspended.");
        }

        TenantUserLoginMethod tenantUserLoginMethod = await _tenantUserLoginMethodRepository.FirstOrDefaultAsync(
            tulm => tulm.TenantUserId == tenantUser.Id && tulm.LoginMethodCode == "PASSWORD",
            ct) ?? throw new UnauthorizedException("Password authentication not available for this user.");

        TenantUserPassword passwordRecord = await _tenantUserPasswordRepository.FirstOrDefaultAsync(
            p => p.TenantUserLoginMethodId == tenantUserLoginMethod.Id, ct) ??
            throw new UnauthorizedException("Invalid credentials.");

        Password password = await _passwordRepository.GetByIdAsync(passwordRecord.PasswordId, ct) ??
            throw new UnauthorizedException("Invalid credentials.");

        if (!_passwordHasher.VerifyPassword(request.Password, password.PasswordHash))
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        tenantUser = await _tenantUserRepository.Query()
            .Include(tu => tu.Tenant)
            .FirstOrDefaultAsync(tu => tu.Id == tenantUser.Id, ct) ??
            throw new UnauthorizedException("Invalid credentials.");

        string accessToken = await _jwtTokenService.GenerateTenantAccessTokenAsync(tenantUser, ct);
        string refreshToken = await _jwtTokenService.GenerateRefreshTokenAsync(ct);

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var tenantUserRefreshToken = new TenantUserRefreshToken
        {
            TenantUserId = tenantUser.Id,
            RefreshTokenId = refreshTokenEntity.Id
        };

        await _tenantUserRefreshTokenRepository.AddAsync(tenantUserRefreshToken, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        TenantUserInfo userInfo = new()
        {
            Id = tenantUser.Id,
            Username = tenantUser.Username,
            Email = tenantUser.Email,
            Name = tenantUser.Name,
            TenantId = tenantUser.TenantId,
            TenantName = tenantUser.Tenant?.Name ?? string.Empty,
        };

        return new TenantLoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = userInfo
        };
    }

    private async Task<TenantUser?> FindTenantUserAsync(string emailOrUsername, Guid? tenantId, CancellationToken ct)
    {
        IQueryable<TenantUser> query = _tenantUserRepository.Query();

        if (tenantId.HasValue)
        {
            query = query.Where(tu => tu.TenantId == tenantId.Value);
        }

        if (emailOrUsername.Contains('@'))
        {
            return await query.FirstOrDefaultAsync(tu => tu.Email == emailOrUsername, ct);
        }

        TenantUser? userByUsername = await query.FirstOrDefaultAsync(tu => tu.Username == emailOrUsername, ct);

        if (userByUsername != null)
        {
            return userByUsername;
        }

        return await query.FirstOrDefaultAsync(tu => tu.Email == emailOrUsername, ct);
    }
}