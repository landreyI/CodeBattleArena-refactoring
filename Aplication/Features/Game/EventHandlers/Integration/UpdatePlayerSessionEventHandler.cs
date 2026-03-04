using AutoMapper;
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.PlayerSessions.Events.Integration;
using MediatR;

namespace CodeBattleArena.Application.Features.Game.EventHandlers.Integration
{
    public class UpdatePlayerSessionEventHandler
        : INotificationHandler<DomainEventNotification<UpdatePlayerSessionIntegrationEvent>>
    {
        private readonly ISessionNotificationService _sessionNotificationService;
        private readonly IMapper _mapper;

        public UpdatePlayerSessionEventHandler(
            ISessionNotificationService sessionNotificationService,
            IMapper mapper)
        {
            _sessionNotificationService = sessionNotificationService;
            _mapper = mapper;
        }

        public async Task Handle(DomainEventNotification<UpdatePlayerSessionIntegrationEvent> notification, CancellationToken ct)
        {
            await _sessionNotificationService.NotifyUpdatePlayerSessionAsync
                (_mapper.Map<PlayerSessionDto>(notification.DomainEvent.PlayerSession));
        }
    }
}
