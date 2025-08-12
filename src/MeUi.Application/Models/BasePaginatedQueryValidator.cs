using FluentValidation;

namespace MeUi.Application.Models;

/// <summary>
/// Validator for base paginated queries
/// </summary>
public class BasePaginatedQueryValidator<TResponse> : AbstractValidator<BasePaginatedQuery<TResponse>>
{
    public BasePaginatedQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must not exceed 100.");

        RuleFor(x => x.Search)
            .MaximumLength(255)
            .When(x => !string.IsNullOrEmpty(x.Search))
            .WithMessage("Search term must not exceed 255 characters.");

        RuleFor(x => x.SortDirection)
            .Must(x => x == null || x.ToLower() == "asc" || x.ToLower() == "desc")
            .WithMessage("Sort direction must be 'asc' or 'desc'.");
    }
}
