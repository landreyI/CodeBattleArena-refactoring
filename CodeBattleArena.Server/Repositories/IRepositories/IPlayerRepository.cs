using CodeBattleArena.Server.DTO;
using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.IRepositories
{
    public interface IPlayerRepository
    {
        Task<Player> GetPlayerAsync(string id, CancellationToken cancellationToken);
        Task<List<Player>> GetPlayersAsync(IFilter<Player>? filter, CancellationToken cancellationToken);
        Task AddVictoryPlayerAsync(string id, CancellationToken cancellationToken);
        Task AddCountGamePlayerAsync(string id, CancellationToken cancellationToken);
        Task<List<Session>> MyGamesListAsync(string id, CancellationToken cancellationToken);
    }
}
