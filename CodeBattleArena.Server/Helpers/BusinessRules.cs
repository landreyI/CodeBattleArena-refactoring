using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Helpers
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
        public static bool IsEditSession(string userId, Session session, IList<string>? roles)
        {
            return IsOwner(userId, session) || IsModerationRole(roles);
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

        public static bool IsOwner(string userId, Session session)
        {
            return session.CreatorId == userId;
        }
        public static bool IsStartetSession(Session session)
        {
            return session.IsStart;
        }
        public static bool IsFinishSession(Session session)
        {
            return session.IsFinish;
        }

        public static PlayerDto ChangeActiveItem(PlayerDto dtoPlayer, Item item)
        {
            switch (item.Type)
            {
                case TypeItem.Background:
                    dtoPlayer.ActiveBackgroundId = item.IdItem;
                    break;
                case TypeItem.Badge:
                    dtoPlayer.ActiveBadgeId = item.IdItem;
                    break;
                case TypeItem.Avatar:
                    dtoPlayer.ActiveAvatarId = item.IdItem;
                    break;
                case TypeItem.Border:
                    dtoPlayer.ActiveBorderId = item.IdItem;
                    break;
                case TypeItem.Title:
                    dtoPlayer.ActiveTitleId = item.IdItem;
                    break;
                default:
                    throw new ArgumentException($"Unknown item type: {item.Type}");
            }
            return dtoPlayer;
        }
    }
}
