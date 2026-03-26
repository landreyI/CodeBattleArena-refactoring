
using Ardalis.Specification;
using CodeBattleArena.Application.Features.Notifications.Filters;
using CodeBattleArena.Application.Features.Notifications.Specifications;
using CodeBattleArena.Domain.Enums;

namespace CodeBattleArena.Application.Features.Items.Specifications
{
    public class NotificationsListSpec : NotificationBaseSpec
    {
        public NotificationsListSpec(Guid playerId, NotificationFilter? filter = default) : base()
        {
            Query.Where(pi => pi.UserId == playerId)
                 .AsNoTracking();

            if(filter != null)
            ApplyFilter(filter);
        }

        private void ApplyFilter(NotificationFilter filter)
        {

            if (!string.IsNullOrWhiteSpace(filter.Type) &&
                Enum.TryParse<NotificationType>(filter.Type, true, out var stateEnum))
            {
                Query.Where(x => x.Type == stateEnum);
            }

            Query.Skip((filter.PageNumber - 1) * filter.PageSize)
                     .Take(filter.PageSize);
        }
    }
}
