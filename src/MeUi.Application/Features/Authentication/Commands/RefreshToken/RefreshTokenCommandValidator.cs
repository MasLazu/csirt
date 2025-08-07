using FluentValidation;

namespace MeUi.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
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
            return false;

        // Check if token contains only valid characters (base64url or similar)
        return System.Text.RegularExpressions.Regex.IsMatch(token, @"^[A-Za-z0-9\-_=+/]+$");
    }
}