using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authentication.Queries.GetActiveLoginMethods;

public class GetActiveLoginMethodsQueryHandler : IRequestHandler<GetActiveLoginMethodsQuery, IEnumerable<LoginMethodDto>>
{
    private readonly IRepository<LoginMethod> _loginMethodRepository;

    public GetActiveLoginMethodsQueryHandler(IRepository<LoginMethod> loginMethodRepository)
    {
        _loginMethodRepository = loginMethodRepository;
    }

    public async Task<IEnumerable<LoginMethodDto>> Handle(GetActiveLoginMethodsQuery request, CancellationToken ct)
    {
        IEnumerable<LoginMethod> loginMethods = await _loginMethodRepository.FindAsync(lm => lm.IsActive, ct);
        return loginMethods.Adapt<IEnumerable<LoginMethodDto>>();
    }
}