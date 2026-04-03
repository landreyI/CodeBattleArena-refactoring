
using AutoMapper;
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Players.Events.Integration;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.EventHandlers.Integration
{
    public class ItemEquippedEventHandler
        : INotificationHandler<DomainEventNotification<PlayerItemEquippedIntegrationEvent>>
    {
        private readonly IPlayerNotificationService _notificationService;
        private readonly IMapper _mapper;

        public ItemEquippedEventHandler(IPlayerNotificationService notificationService, IMapper mapper)
        {
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task Handle(DomainEventNotification<PlayerItemEquippedIntegrationEvent> notification, CancellationToken ct)
        {
            var player = notification.DomainEvent.Player;
            var item = notification.DomainEvent.Item;
            var itemDto = _mapper.Map<ItemDto>(item);

            await _notificationService.NotifyItemEquippedAsync(player.Id, itemDto);
        }
    }
}
