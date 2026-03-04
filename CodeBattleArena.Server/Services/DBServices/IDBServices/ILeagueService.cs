using CodeBattleArena.Server.Helpers;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Untils;


namespace CodeBattleArena.Server.Services.DBServices.IDBServices
{
    public interface ILeagueService
    {
        Task<List<PlayersLeague>> GetPlayersLeagues(CancellationToken cancellationToken);
        Task<League> GetLeagueByPlayerAsync(string idPlayer, CancellationToken cancellationToken);
        Task<League> GetLeagueByNameAsync(string name, CancellationToken cancellationToken);
        Task<League> GetLeagueAsync(int id, CancellationToken cancellationToken);
        Task<List<League>> GetLeaguesAsync(CancellationToken cancellationToken);
        Task<Result<Unit, ErrorResponse>> AddLeagueInDbAsync(League league, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> DeleteLeagueInDbAsync(int id, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> UpdateLeagueInDbAsync(League league, CancellationToken cancellationToken, bool commit = true);
    }
}
