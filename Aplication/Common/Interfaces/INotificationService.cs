
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;

namespace CodeBattleArena.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task<Result> SendNotificationAsync(Guid UserId, string content, NotificationType type, Guid? relatedEntityId = default, CancellationToken ct = default);
    }
}
