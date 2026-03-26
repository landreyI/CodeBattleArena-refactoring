using CodeBattleArena.Domain.Enums;

namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public record NotificationDto(
        Guid Id,
        string Content,
        NotificationType Type,
        DateTime CreatedAt,
        bool IsRead
    );
}
