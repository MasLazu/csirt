using Mapster;
using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.Authentication.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Authentication.Queries.GetUserLoginMethods;

public class GetUserLoginMethodsQueryHandler : IRequestHandler<GetUserLoginMethodsQuery, IEnumerable<LoginMethodDto>>
{
    private readonly IRepository<UserLoginMethod> _userLoginMethodRepository;
    private readonly IRepository<LoginMethod> _loginMethodRepository;

    public GetUserLoginMethodsQueryHandler(
        IRepository<UserLoginMethod> userLoginMethodRepository,
        IRepository<LoginMethod> loginMethodRepository)
    {
        _userLoginMethodRepository = userLoginMethodRepository;
        _loginMethodRepository = loginMethodRepository;
    }

    public async Task<IEnumerable<LoginMethodDto>> Handle(GetUserLoginMethodsQuery request, CancellationToken ct)
    {
        IEnumerable<string> userLoginMethodCodes = (await _userLoginMethodRepository
            .FindAsync(ulm => ulm.UserId == request.UserId && !ulm.IsDeleted, ct))
            .Select(ulm => ulm.LoginMethodCode);

        IEnumerable<LoginMethod> loginMethods = await _loginMethodRepository
            .FindAsync(lm => userLoginMethodCodes.Contains(lm.Code) && lm.IsActive && !lm.IsDeleted, ct);

        return loginMethods.Adapt<IEnumerable<LoginMethodDto>>();
    }
}