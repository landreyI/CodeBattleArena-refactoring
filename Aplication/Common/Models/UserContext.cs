using CodeBattleArena.Domain.Enums;

namespace CodeBattleArena.Application.Common.Models
{
    public record UserContext(Guid? PlayerId, string? IdentityId, IList<string> Roles)
    {
        public bool IsAuthenticated => PlayerId.HasValue && !string.IsNullOrWhiteSpace(IdentityId);

        public bool IsAdmin => Roles.Contains(Role.Admin.ToString());
        public bool IsModerator => Roles.Contains(Role.Moderator.ToString());
        public bool IsManager => Roles.Contains(Role.Manager.ToString());
        public bool IsBanned => Roles.Contains(Role.Banned.ToString());

        public bool IsStaff => IsAdmin || IsModerator || IsManager;
    }
}
