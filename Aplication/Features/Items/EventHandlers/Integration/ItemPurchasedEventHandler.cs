
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Players.Events.Integration;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.EventHandlers.Integration
{
    public class ItemPurchasedEventHandler
        : INotificationHandler<DomainEventNotification<PlayerItemPurchasedIntegrationEvent>>
    {
        private readonly INotificationService _notificationService;

        public ItemPurchasedEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(DomainEventNotification<PlayerItemPurchasedIntegrationEvent> notification, CancellationToken ct)
        {
        }
    }
}
