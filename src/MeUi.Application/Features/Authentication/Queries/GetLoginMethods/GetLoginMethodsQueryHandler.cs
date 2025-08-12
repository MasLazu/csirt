using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authentication.Queries.GetLoginMethods;

public class GetActiveLoginMethodsQueryHandler : IRequestHandler<GetLoginMethodsQuery, IEnumerable<LoginMethodDto>>
{
    private readonly IRepository<LoginMethod> _loginMethodRepository;

    public GetActiveLoginMethodsQueryHandler(IRepository<LoginMethod> loginMethodRepository)
    {
        _loginMethodRepository = loginMethodRepository;
    }

    public async Task<IEnumerable<LoginMethodDto>> Handle(GetLoginMethodsQuery request, CancellationToken ct)
    {
        IEnumerable<LoginMethod> loginMethods = await _loginMethodRepository.GetAllAsync(ct);
        return loginMethods.Adapt<IEnumerable<LoginMethodDto>>();
    }
}