
namespace CodeBattleArena.Application.Features.Notifications.Filters
{
    public class NotificationFilter
    {
        public string? Type { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }
}
