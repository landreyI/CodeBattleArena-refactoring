using Microsoft.AspNetCore.Authorization;

namespace CodeBattleArena.Application.Common.Security
{
    public record CanModerationRequirement : IAuthorizationRequirement;
}
