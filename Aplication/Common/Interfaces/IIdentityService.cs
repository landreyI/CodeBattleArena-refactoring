
using CodeBattleArena.Application.Common.Models;

namespace CodeBattleArena.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<UserContext> GetUserContextAsync();
        Task<(string UserId, bool Succeeded)> GetOrCreateExternalUserAsync(string email, string provider, string providerKey, string name, string picture);
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<string> CreateUserAsync(string email, string password);
        Task<(string UserId, string Email, IList<string> Roles)?> AuthenticateAsync(string email, string password);
        Task<bool> CheckPasswordAsync(string identityId, string password);
        Task<bool> CheckPasswordByEmailAsync(string email, string password);
        Task<string?> GetUserNameAsync(string identityId);
        Task<string?> GetUserEmailByIdAsync(string identityId);
        string? CurrentIdentityId();
        Guid? CurrentPlayerId();
    }
}
