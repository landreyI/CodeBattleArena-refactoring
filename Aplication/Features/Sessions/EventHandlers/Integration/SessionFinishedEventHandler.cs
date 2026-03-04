using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Domain.Sessions.Events.Integration;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.EventHandlers.Integration
{
    public class SessionFinishedEventHandler
        : INotificationHandler<DomainEventNotification<SessionFinishedIntegrationEvent>>
    {
        private readonly ISessionNotificationService _sessionNotificationService;

        public SessionFinishedEventHandler(ISessionNotificationService sessionNotificationService)
        {
            _sessionNotificationService = sessionNotificationService;
        }

        public async Task Handle(DomainEventNotification<SessionFinishedIntegrationEvent> notification, CancellationToken ct)
        {
            await _sessionNotificationService.NotifyFinishGameAsync(notification.DomainEvent.Session.Id);
        }
    }
}
