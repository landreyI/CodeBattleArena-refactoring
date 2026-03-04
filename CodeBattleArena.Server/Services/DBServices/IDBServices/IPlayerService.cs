using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Untils;

namespace CodeBattleArena.Server.Services.DBServices.IDBServices
{
    public interface IPlayerService
    {
        Task<Result<(PlayerDto Player, bool IsEdit), ErrorResponse>> GetPlayerInfoAsync(string targetId, string requesterId, CancellationToken cancellationToken);
        Task<Result<Unit, ErrorResponse>> ChangeActiveItem(string targetId, string requesterId, int idItem, CancellationToken ct, bool commit = true);
        Task<Result<Unit, ErrorResponse>> UpdatePlayerAsync(string authUserId, PlayerDto dto, CancellationToken ct);
        Task<Result<Unit, ErrorResponse>> SelectRolesAsync(string idPlayer, string[] roles, CancellationToken ct);
        Task<bool> IsUserNameTakenAsync(string username, string id);
        Task<IList<string>> GetRolesAsync(string userId);
        Task<IList<string>> GetRolesAsync(Player user);
        Task<Player> GetPlayerAsync(string id, CancellationToken ct);
        Task<List<Player>> GetPlayersAsync(IFilter<Player>? filter, CancellationToken ct);
        Task<Result<Unit, ErrorResponse>> AddVictoryPlayerInDbAsync(string id, CancellationToken ct, bool commit = true);
        Task<List<Session>> MyGamesListAsync(string id, CancellationToken ct);
        Task<Result<Unit, ErrorResponse>> UpdatePlayerInDbAsync(Player player);
    }
}
