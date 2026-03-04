using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.Judge0;
using CodeBattleArena.Server.Specifications;
using CodeBattleArena.Server.Untils;

namespace CodeBattleArena.Server.Services.DBServices.IDBServices
{
    public interface IPlayerSessionService
    {
        Task<Result<PlayerSession, ErrorResponse>> CreatPlayerSession(string idPlayer, int idSession, CancellationToken ct, bool commit = true);
        Task<Result<Unit, ErrorResponse>> KickOutSessionAsync(string idAuthPlayer, int idSession, string idDeletePlayer, CancellationToken ct, bool commit = true);
        Task<Session> GetActiveSession(string idPlayer, CancellationToken ct);
        Task<Result<PlayerSession, ErrorResponse>> SaveCheckCodeAsync(int idSession, string idPlayer, string code, ExecutionResult executionResult, CancellationToken ct, bool commit = true);
        Task<Result<Unit, ErrorResponse>> InviteSessionAsync(string authUserId, List<string> idPlayersInvite, CancellationToken ct);
        Task<Result<Unit, ErrorResponse>> FinishTask(int idSession, string idPlayer, CancellationToken ct, bool commit = true);
        Task<Result<Unit, ErrorResponse>> AddPlayerSessionInDbAsync(PlayerSession playerSession, CancellationToken ct, bool commit = true);
        Task<PlayerSession> GetPlayerSessionAsync(ISpecification<PlayerSession> spec, CancellationToken ct);
        Task<List<PlayerSession>> GetListPlayerSessionAsync(ISpecification<PlayerSession> spec, CancellationToken ct);
        Task<Result<Unit, ErrorResponse>> UpdatePlayerSessionInDbAsync(PlayerSession playerSession, CancellationToken ct, bool commit = true);
        Task<Result<Unit, ErrorResponse>> FinishTaskInDbAsync(int idSession, string idPlayer, CancellationToken ct, bool commit = true);
        Task<Result<Unit, ErrorResponse>> DelPlayerSessionInDbAsync(int idSession, string idPlayer, CancellationToken ct, bool commit = true);
    }
}
