using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Notifications;
using MediatR;

namespace CodeBattleArena.Application.Features.Notifications.Queries.GetNotification
{
    public record GetNotificationQuery(Guid Id) : IRequest<Result<Notification>>, ICacheInvalidator
    {
        // Удаляем ВСЕ списки, потому что состав изменился
        public string[] Tags => [Common.CacheKeys.Notification.ListTag];
    }
}
