
using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Sessions.Interfaces;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Players;
using CodeBattleArena.Domain.Sessions;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Commands.KickOutSession
{
    public class KickOutSessionHandler : IRequestHandler<KickOutSessionCommand, Result<bool>>
    {
        private readonly IRepository<Player> _playerRepository;
        private readonly ISessionAccessService _accessService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISessionNotificationService _sessionNotificationService;
        private readonly IMapper _mapper;

        public KickOutSessionHandler(
            IRepository<Session> sessionRepository,
            IRepository<Player> playerRepository,
            IIdentityService identityService,
            ISessionAccessService accessService,
            IUnitOfWork unitOfWork,
            ISessionNotificationService sessionNotificationService,
            IMapper mapper)
        {
            _playerRepository = playerRepository;
            _accessService = accessService;
            _unitOfWork = unitOfWork;
            _sessionNotificationService = sessionNotificationService;
            _mapper = mapper;
        }
        public async Task<Result<bool>> Handle(KickOutSessionCommand request, CancellationToken cancellationToken)
        {
            var sessionResult = await _accessService.GetSessionForUpdateAsync(request.SessionId, cancellationToken);

            if (sessionResult.IsFailure)
                return Result<bool>.Failure(sessionResult.Error);

            var session = sessionResult.Value;

            var resultRemovePlayer = session.RemovePlayer(request.PlayerId);
            if (resultRemovePlayer.IsFailure)
                return Result<bool>.Failure(resultRemovePlayer.Error);

            await _unitOfWork.CommitAsync(cancellationToken);

            var player = await _playerRepository.GetByIdAsync(request.PlayerId, asNoTracking: true, cancellationToken);
            if (player == null)
                return Result<bool>.Failure(new Error("player.not_found", "Player not found", 404));

            var dtoPlayer = _mapper.Map<PlayerDto>(player);

            await _sessionNotificationService.NotifySessionKickOutAsync(request.SessionId, dtoPlayer);

            return Result<bool>.Success(true);
        }
    }
}
