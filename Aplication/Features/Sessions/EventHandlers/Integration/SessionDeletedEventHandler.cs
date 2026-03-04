using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Domain.Sessions.Events.Integration;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.EventHandlers.Integration
{
    public class SessionDeletedEventHandler
        : INotificationHandler<DomainEventNotification<SessionDeletedIntegrationEvent>>
    {
        private readonly ISessionNotificationService _sessionNotificationService;

        public SessionDeletedEventHandler(ISessionNotificationService sessionNotificationService)
        {
            _sessionNotificationService = sessionNotificationService;
        }

        public async Task Handle(DomainEventNotification<SessionDeletedIntegrationEvent> notification, CancellationToken ct)
        {
            await _sessionNotificationService.NotifySessionDeletedGroupAsync(notification.DomainEvent.Id);
            await _sessionNotificationService.NotifySessionDeletedAllAsync(notification.DomainEvent.Id);
        }
    }
}
