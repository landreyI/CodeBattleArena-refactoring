using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Sessions.Queries.GetSession;
using CodeBattleArena.Application.Features.Sessions.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Sessions;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetActiveSession
{
    public class GetActiveSessionHandler : IRequestHandler<GetActiveSessionQuery, Result<SessionDto?>>
    {
        private readonly IRepository<Session> _sessionRepository;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;

        public GetActiveSessionHandler(IRepository<Session> sessionRepository, IMapper mapper, IIdentityService identityService, IMediator mediator)
        {
            _sessionRepository = sessionRepository;
            _mapper = mapper;
            _identityService = identityService;
            _mediator = mediator;
        }

        public async Task<Result<SessionDto?>> Handle(GetActiveSessionQuery request, CancellationToken cancellationToken)
        {
            var currentPlayerId = _identityService.CurrentPlayerId();
            if (!currentPlayerId.HasValue)
                return Result<SessionDto?>.Failure(new Error("auth.unauthorized", "User not found in context", 401));

            var sessionId = await _sessionRepository.GetIdBySpecAsync(new ActiveSessionIdSpec(currentPlayerId.Value), cancellationToken);
            if (sessionId == Guid.Empty)
                return Result<SessionDto?>.Failure(new Error("session.active", "Active session not found", 200));

            var sessionResult = await _mediator.Send(new GetSessionDataQuery(sessionId.Value), cancellationToken);

            if (sessionResult.IsFailure)
                return Result<SessionDto?>.Failure(sessionResult.Error);

            return Result<SessionDto?>.Success(_mapper.Map<SessionDto>(sessionResult.Value));
        }
    }
}
