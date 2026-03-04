using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CodeBattleArena.Server.Helpers;

namespace CodeBattleArena.Server.Infrastructure.Attributes
{
    public class RequireModerationRoleAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new ForbidResult();
                return;
            }

            var roles = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if (!BusinessRules.IsModerationRole(roles))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
