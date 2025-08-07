using FluentValidation;

namespace MeUi.Application.Features.Authentication.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.EmailOrUsername)
            .NotEmpty()
            .WithMessage("Email or Username is required.")
            .MaximumLength(255)
            .WithMessage("Email or Username must not exceed 255 characters.")
            .Must(BeValidEmailOrUsername)
            .WithMessage("Must be a valid email address or username.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(128)
            .WithMessage("Password must not exceed 128 characters.");
    }

    private static bool BeValidEmailOrUsername(string emailOrUsername)
    {
        if (string.IsNullOrWhiteSpace(emailOrUsername))
            return false;

        // Check if it's a valid email
        if (emailOrUsername.Contains('@'))
        {
            return IsValidEmail(emailOrUsername);
        }

        // Check if it's a valid username (alphanumeric, dots, hyphens, underscores)
        return System.Text.RegularExpressions.Regex.IsMatch(emailOrUsername, @"^[a-zA-Z0-9._-]+$");
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}