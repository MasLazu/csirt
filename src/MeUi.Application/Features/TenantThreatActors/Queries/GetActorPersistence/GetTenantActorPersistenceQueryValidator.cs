using FluentValidation;

namespace MeUi.Application.Features.TenantThreatActors.Queries.GetActorPersistence;

public class GetTenantActorPersistenceQueryValidator : AbstractValidator<GetTenantActorPersistenceQuery>
{
    public GetTenantActorPersistenceQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.Start)
            .LessThan(x => x.End)
            .WithMessage("Start must be before End.");
    }
}