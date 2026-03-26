using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.PlayerItems.Events.Integration;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.EventHandlers.Integration
{
    public class ItemReceivedEventHandler : INotificationHandler<DomainEventNotification<ItemReceivedIntegrationEvent>>
    {
        private readonly INotificationService _notificationService;

        public ItemReceivedEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(DomainEventNotification<ItemReceivedIntegrationEvent> notification, CancellationToken ct)
        {
            var playerItem = notification.DomainEvent.PlayerItem;
            bool isGift = playerItem.PlayerId != playerItem.PayerId;

            if (isGift)
            {
                // Получателю
                await _notificationService.SendNotificationAsync(
                    playerItem.PlayerId,
                    "You were given a gift!",
                    NotificationType.Gift,
                    playerItem.Id,
                    ct);

                // Отправителю
                await _notificationService.SendNotificationAsync(
                    playerItem.PayerId,
                    "You have successfully sent the gift.",
                    NotificationType.Gift,
                    playerItem.Id,
                    ct);
            }
            else
            {
                // Обычная покупка самому себе
                await _notificationService.SendNotificationAsync(
                    playerItem.PlayerId,
                    "Congratulations on your new item!",
                    NotificationType.Purchase,
                    playerItem.Id,
                    ct);
            }
        }
    }
}
