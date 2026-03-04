using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Specifications;

namespace CodeBattleArena.Server.IRepositories
{
    public interface IPlayerSessionRepository
    {
        Task AddPlayerSessionAsync(PlayerSession playerSession, CancellationToken cancellationToken);
        Task<PlayerSession> GetPlayerSessionAsync(ISpecification<PlayerSession> spec, CancellationToken cancellationToken);
        Task<List<PlayerSession>> GetListPlayerSessionByIdAsync(ISpecification<PlayerSession> spec, CancellationToken cancellationToken);

        Task UpdatePlayerSession(PlayerSession playerSession);
        Task FinishTaskAsync(int idSession, string idPlayer, CancellationToken cancellationToken);
        Task DelPlayerSessionAsync(int idSession, string idPlayer, CancellationToken cancellationToken);
    }
}
