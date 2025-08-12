using System.Text.RegularExpressions;
using FluentValidation;

namespace MeUi.Application.Features.Authentication.Commands.TokenRefresh;

public class RefreshTokenCommandValidator : AbstractValidator<TokenRefreshCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required.")
            .MinimumLength(32)
            .WithMessage("Refresh token must be at least 32 characters long.")
            .MaximumLength(512)
            .WithMessage("Refresh token must not exceed 512 characters.")
            .Must(BeValidTokenFormat)
            .WithMessage("Refresh token format is invalid.");
    }

    private static bool BeValidTokenFormat(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        return Regex.IsMatch(token, @"^[A-Za-z0-9\-_=+/]+$");
    }
}