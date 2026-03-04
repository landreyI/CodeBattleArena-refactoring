
namespace CodeBattleArena.Application.Common.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userId, string email, Guid playerId, IList<string> roles);
        string GenerateRefreshToken();
    }
}
