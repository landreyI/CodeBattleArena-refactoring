
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.Sessions;

namespace CodeBattleArena.Application.Common.Helpers
{
    public static class BusinessRules
    {
        public static bool IsStaffRole(IList<string>? roles)
        {
            return IsModerationRole(roles) || IsEditRole(roles);
        }
        public static bool IsModerationRole(IList<string>? roles)
        {
            return roles != null && roles
                .Any(r => Enum.TryParse<Role>(r, true, out var parsedRole) &&
                          (parsedRole == Role.Admin || parsedRole == Role.Moderator));
        }
        public static bool IsEditSession(Guid userId, Session session, IList<string>? roles)
        {
            return IsOwner(userId, session) || IsModerationRole(roles);
        }
        public static bool IsStartetSession(Session session)
        {
            return session.Status == GameStatus.Waiting;
        }
        public static bool IsEditRole(IList<string>? roles)
        {
            return roles != null && roles
                .Any(r => Enum.TryParse<Role>(r, true, out var parsedRole) &&
                          (parsedRole == Role.Admin || parsedRole == Role.Manager));
        }
        public static bool IsBannedRole(IList<string>? roles)
        {
            return roles != null && roles
                .Any(r => Enum.TryParse<Role>(r, true, out var parsedRole) &&
                          (parsedRole == Role.Banned));
        }

        public static bool IsOwner(Guid userId, Session session)
        {
            return session.CreatorId == userId;
        }
    }
}
