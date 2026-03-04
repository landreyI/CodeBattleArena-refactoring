using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;

namespace CodeBattleArena.Domain.Notifications
{
    public class Notification : BaseEntity<Guid>
    {
        public Guid UserId { get; private set; }
        public string Content { get; private set; } = string.Empty;
        public NotificationType Type { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsRead { get; private set; }
        public Guid? RelatedEntityId { get; private set; }
        public DateTime? ReadAt { get; private set; }

        // Конструктор для EF Core
        private Notification() { }

        private Notification(Guid userId, string content, NotificationType type, Guid? relatedEntityId = default)
        {
            UserId = userId;
            Content = content;
            Type = type;
            CreatedAt = DateTime.UtcNow;
            IsRead = false;
            RelatedEntityId = relatedEntityId;
        }

        public static Result<Notification> Create(Guid userId, string content, NotificationType type, Guid? relatedEntityId = default)
        {
            if (string.IsNullOrWhiteSpace(content))
                return Result<Notification>.Failure(new Error("Notification.Content", "Content cannot be empty."));

            if(relatedEntityId.HasValue && relatedEntityId == Guid.Empty)
                return Result<Notification>.Failure(new Error("Notification.RelatedEntityId", "RelatedEntityId empty"));

            return Result<Notification>.Success(new Notification(userId, content, type, relatedEntityId));
        }

        public void MarkAsRead()
        {
            if (!IsRead)
            {
                IsRead = true;
                ReadAt = DateTime.UtcNow;
            }
        }
    }
}
