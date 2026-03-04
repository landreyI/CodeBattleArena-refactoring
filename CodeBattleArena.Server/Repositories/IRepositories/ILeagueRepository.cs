using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Repositories.IRepositories
{
    public interface ILeagueRepository
    {
        Task<League> GetLeagueAsync(int id, CancellationToken cancellationToken);
        Task<League> GetLeagueByPlayerAsync(string idPlayer, CancellationToken cancellationToken);
        Task<League> GetLeagueByNameAsync(string name, CancellationToken cancellationToken);
        Task<List<League>> GetLeaguesAsync(CancellationToken cancellationToken);
        Task AddLeagueAsync(League league, CancellationToken cancellationToken);
        Task DeleteLeagueAsync(int id, CancellationToken cancellationToken);
        Task UpdateLeague(League league);
    }
}
