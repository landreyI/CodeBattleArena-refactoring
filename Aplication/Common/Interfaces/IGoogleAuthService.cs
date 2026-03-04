using CodeBattleArena.Application.Features.Auth.Models;

namespace CodeBattleArena.Application.Common.Interfaces
{
    public interface IGoogleAuthService
    {
        string GenerateOauthRequestUrl();
        Task<GoogleTokenResponse?> ExchangeCodeOnTokenAsync(string code, CancellationToken ct);
        Task<GoogleUserInfoResponse?> GetUserInfoAsync(string accessToken, CancellationToken ct);
    }
}
