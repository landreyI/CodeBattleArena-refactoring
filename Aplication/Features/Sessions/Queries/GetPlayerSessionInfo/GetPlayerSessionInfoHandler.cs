using AutoMapper;
using CodeBattleArena.Application.Common.Helpers;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Sessions.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.PlayerSessions;
using CodeBattleArena.Domain.Sessions;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetPlayerSessionInfo
{
    public class GetPlayerSessionInfoHandler : IRequestHandler<GetPlayerSessionInfoQuery, Result<PlayerSessionDto?>>
    {
        private readonly IRepository<Session> _sessionRepository;
        private readonly IRepository<PlayerSession> _playerSessionRepository;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public GetPlayerSessionInfoHandler(
            IRepository<Session> sessionRepository, 
            IRepository<PlayerSession> palyerSessionRepository, 
            IMapper mapper, 
            IIdentityService identityService)
        {
            _sessionRepository = sessionRepository;
            _playerSessionRepository = palyerSessionRepository;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<PlayerSessionDto?>> Handle(GetPlayerSessionInfoQuery request, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetUserContextAsync();

            var sessionSpec = new SessionReadOnlySpec(request.SessionId);
            var session = await _sessionRepository.GetBySpecAsync(sessionSpec, cancellationToken);
            if (session is null)
                return Result<PlayerSessionDto?>.Failure(new Error("session.not_found", "Session not found", 404));

            // Проверяем права
            if (!SessionAccessPolicy.CanViewPlayerDetails(session, request.PlayerId, user))
            {
                var domainAccess = session.CheckViewAccess(user.PlayerId, request.PlayerId);
                if (domainAccess.IsFailure)
                    return Result<PlayerSessionDto?>.Failure(domainAccess.Error);
            }

            // Загружаем данные конкретного игрока
            var playerSpec = new PlayerSessionIncludesSpec(request.SessionId, request.PlayerId);
            var playerSession = await _playerSessionRepository.GetBySpecAsync(playerSpec, cancellationToken);

            if (playerSession is null)
                return Result<PlayerSessionDto?>.Failure(new Error("player_session.not_found", "Info not found", 404));

            return Result<PlayerSessionDto?>.Success(_mapper.Map<PlayerSessionDto>(playerSession));
        }
    }
}
