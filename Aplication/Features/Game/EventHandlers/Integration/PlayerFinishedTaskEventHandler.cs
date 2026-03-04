using AutoMapper;
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Sessions.Specifications;
using CodeBattleArena.Domain.Players;
using CodeBattleArena.Domain.PlayerSessions.Events.Integration;
using CodeBattleArena.Domain.Sessions;
using MediatR;

namespace CodeBattleArena.Application.Features.Game.EventHandlers.Integration
{
    public class PlayerFinishedTaskEventHandler
        : INotificationHandler<DomainEventNotification<PlayerFinishedTaskIntegrationEvent>>
    {
        private readonly IRepository<Player> _playerRepository;
        private readonly IRepository<Session> _sessionRepository;
        private readonly ISessionNotificationService _sessionNotificationService;
        private readonly IMapper _mapper;

        public PlayerFinishedTaskEventHandler(
            IRepository<Session> sessionRepository,
            IRepository<Player> playerRepository,
            ISessionNotificationService sessionNotificationService,
            IMapper mapper)
        {
            _sessionRepository = sessionRepository;
            _playerRepository = playerRepository;
            _sessionNotificationService = sessionNotificationService;
            _mapper = mapper;
        }

        public async Task Handle(DomainEventNotification<PlayerFinishedTaskIntegrationEvent> notification, CancellationToken ct)
        {
            var sessionId = notification.DomainEvent.SessionId;

            // Получаем актуальный счетчик
            var spec = new SessionReadOnlySpec(sessionId);
            var session = await _sessionRepository.GetBySpecAsync(spec, ct);

            if (session != null)
            {
                var countResult = session.GetCompletionCount();
                await _sessionNotificationService.NotifyUpdateCompletedCount(sessionId, countResult.Value!.CompletedCount);
            }

            /*var player = await _playerRepository.GetByIdAsync(notification.DomainEvent.PlayerId, asNoTracking: true, ct);
            if (player == null) return;

            var dtoPlayer = _mapper.Map<PlayerDto>(player);*/
        }
    }
}
