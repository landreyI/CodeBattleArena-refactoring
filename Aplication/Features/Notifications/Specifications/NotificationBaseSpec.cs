
using Ardalis.Specification;
using CodeBattleArena.Domain.Notifications;

namespace CodeBattleArena.Application.Features.Notifications.Specifications
{
    public abstract class NotificationBaseSpec : Specification<Notification>
    {
        protected NotificationBaseSpec() { }

        protected NotificationBaseSpec(Guid Id) : this()
        {
            Query.Where(s => s.Id == Id);
        }
    }
}
