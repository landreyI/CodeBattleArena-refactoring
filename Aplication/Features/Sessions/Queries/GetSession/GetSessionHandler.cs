using AutoMapper;
using CodeBattleArena.Application.Common.Helpers;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Sessions.Models;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSession
{
    public class GetSessionHandler : IRequestHandler<GetSessionQuery, Result<SessionDtoAndIsEdit>>
    {
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;

        public GetSessionHandler(IMapper mapper, IIdentityService identityService, IMediator mediator)
        {
            _mapper = mapper;
            _identityService = identityService;
            _mediator = mediator;
        }
        public async Task<Result<SessionDtoAndIsEdit>> Handle(GetSessionQuery request, CancellationToken ct)
        {
            var sessionResult = await _mediator.Send(new GetSessionDataQuery(request.Id), ct);
            if (sessionResult.IsFailure)
                return Result<SessionDtoAndIsEdit>.Failure(sessionResult.Error);

            var user = await _identityService.GetUserContextAsync();

            bool isEdit = SessionAccessPolicy.CanEditSession(sessionResult.Value, user);

            return Result<SessionDtoAndIsEdit>.Success(new SessionDtoAndIsEdit(_mapper.Map<SessionDto>(sessionResult.Value), isEdit));
        }
    }
}
