using CodeBattleArena.Application.Common.Helpers;
using CodeBattleArena.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CodeBattleArena.Infrastructure.Identity.Handlers
{
    public class CanEditHandler : AuthorizationHandler<CanEditRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanEditRequirement requirement)
        {
            var roles = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if (BusinessRules.IsEditRole(roles))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
