using CodeBattleArena.Server.Enums;
using Microsoft.AspNetCore.Authorization;

namespace CodeBattleArena.Server.Infrastructure.Attributes
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params Role[] roles)
        {
            Roles = string.Join(",", roles.Select(r => r.ToString()));
        }
    }
}
