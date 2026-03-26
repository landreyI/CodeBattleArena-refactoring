
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Notifications.Filters;

namespace CodeBattleArena.Application.Features.Notifications.Queries.GetNotificationsList
{
    public record GetNotificationsListQuery(Guid PlayerId, NotificationFilter Filter) : PagedQueryBase<NotificationDto>
    {
        public override string CacheKey => CacheKeys.Notification.PlayerList(PlayerId, Filter);

        public override string[] Tags => [CacheKeys.Notification.ListTag];
    }
}
