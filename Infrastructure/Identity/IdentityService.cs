using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CodeBattleArena.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserContext> GetUserContextAsync()
        {
            var identityId = CurrentIdentityId();
            var playerId = CurrentPlayerId();
            var roles = string.IsNullOrWhiteSpace(identityId)
                ? new List<string>()
                : await GetUserRolesAsync(identityId);

            return new UserContext(playerId, identityId, roles);
        }

        public string? CurrentIdentityId() => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public Guid? CurrentPlayerId()
        {
            var id = _httpContextAccessor.HttpContext?.User?.FindFirst(c => c.Type == "PlayerId")?.Value;
            return id != null ? Guid.Parse(id) : null;
        }

        public async Task<(string UserId, bool Succeeded)> GetOrCreateExternalUserAsync(string email, string provider, string providerKey, string name, string picture)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded) return (string.Empty, false);

                await _userManager.AddToRolesAsync(user, [Role.User.ToString()]);
            }

            // Привязываем провайдера (Google), если еще не привязан
            var logins = await _userManager.GetLoginsAsync(user);
            if (logins.All(l => l.ProviderKey != providerKey))
            {
                await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerKey, provider));
            }

            return (user.Id, true);
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null ? await _userManager.GetRolesAsync(user) : new List<string>();
        }

        public async Task<(string UserId, string Email, IList<string> Roles)?> AuthenticateAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            var result = await _userManager.CheckPasswordAsync(user, password);
            if (!result) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return (user.Id, user.Email!, roles);
        }

        public async Task<string> CreateUserAsync(string email, string password)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Identity error: {errors}");
            }

            return user.Id;
        }

        public async Task<bool> CheckPasswordAsync(string identityId, string password)
        {
            var user = await _userManager.FindByIdAsync(identityId);
            return user != null && await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<bool> CheckPasswordByEmailAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null && await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<string?> GetUserNameAsync(string identityId)
        {
            var user = await _userManager.FindByIdAsync(identityId);
            return user?.UserName;
        }

        public async Task<string?> GetUserEmailByIdAsync(string identityId)
        {
            var user = await _userManager.FindByIdAsync(identityId);
            return user?.Email;
        }
    }
}