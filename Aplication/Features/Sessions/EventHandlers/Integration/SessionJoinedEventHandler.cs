using AutoMapper;
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Players;
using CodeBattleArena.Domain.Sessions.Events.Integration;
using MediatR;


namespace CodeBattleArena.Application.Features.Sessions.EventHandlers.Integration
{
    public class SessionJoinedEventHandler
        : INotificationHandler<DomainEventNotification<SessionJoinedIntegrationEvent>>
    {
        private readonly IRepository<Player> _playerRepository;
        private readonly ISessionNotificationService _sessionNotificationService;
        private readonly IMapper _mapper;

        public SessionJoinedEventHandler(
            IRepository<Player> playerRepository,
            ISessionNotificationService sessionNotificationService,
            IMapper mapper)
        {
            _playerRepository = playerRepository;
            _sessionNotificationService = sessionNotificationService;
            _mapper = mapper;
        }

        public async Task Handle(DomainEventNotification<SessionJoinedIntegrationEvent> notification, CancellationToken ct)
        {
            var player = await _playerRepository.GetByIdAsync(notification.DomainEvent.PlayerId, asNoTracking: true, ct);
            if (player == null) return;

            var dtoPlayer = _mapper.Map<PlayerDto>(player);
            await _sessionNotificationService.NotifySessionJoinAsync(notification.DomainEvent.SessionId, dtoPlayer);
        }
    }
}
