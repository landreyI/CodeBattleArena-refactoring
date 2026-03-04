using CodeBattleArena.Application.Common.Models;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.Sessions;

namespace CodeBattleArena.Application.Common.Helpers
{
    public static class SessionAccessPolicy
    {
        public static bool CanEditSession(Session session, UserContext user)
        {
            if (user.IsBanned) return false;

            return user.IsStaff || (session.CreatorId == user.PlayerId && session.Status == GameStatus.Waiting);
        }

        public static bool CanViewPlayerDetails(Session session, Guid targetPlayerId, UserContext user)
        {
            if (user.IsStaff) return true;

            return user.PlayerId == targetPlayerId;
        }
    }
}
