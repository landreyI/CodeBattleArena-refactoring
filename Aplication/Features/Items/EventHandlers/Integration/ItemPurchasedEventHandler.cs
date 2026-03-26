
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Items.Events.Integration;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.EventHandlers.Integration
{
    public class ItemPurchasedEventHandler
        : INotificationHandler<DomainEventNotification<ItemPurchasedIntegrationEvent>>
    {
        private readonly INotificationService _notificationService;

        public ItemPurchasedEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(DomainEventNotification<ItemPurchasedIntegrationEvent> notification, CancellationToken ct)
        {
        }
    }
}
