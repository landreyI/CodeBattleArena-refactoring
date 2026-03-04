using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Specifications;
using CodeBattleArena.Server.Untils;

namespace CodeBattleArena.Server.Services.DBServices.IDBServices
{
    public interface ISessionService
    {
        Task<Result<Unit, ErrorResponse>> StartGameAsync(int sessionId, string userId, CancellationToken ct, bool commit = true);
        Task<Result<Unit, ErrorResponse>> FinishGameAsync(int sessionId, string userId, CancellationToken ct, bool? isBackground = false);
        Task<Result<bool, ErrorResponse>> CanAccessSessionPlayersAsync(int sessionId, string userId, CancellationToken ct);
        Task<Result<bool, ErrorResponse>> CanEditSessionAsync(int sessionId, string userId, CancellationToken ct);
        Task<Result<Session, ErrorResponse>> CreateSessionAsync(string userId, SessionDto dto, CancellationToken ct);
        Task<Result<Unit, ErrorResponse>> UpdateSessionAsync(string userId, SessionDto dto, CancellationToken ct);
        Task<Result<SessionDto, ErrorResponse>> SelectTaskForSessionAsync(string userId, int idSession, int idTask, CancellationToken ct);
        Task<Result<Unit, ErrorResponse>> DeletingSessionAsync(string userId, int idSession, CancellationToken ct);
        Task<Result<bool, ErrorResponse>> CheckPasswordAsync(string password, int idSession, CancellationToken ct);
        Task<Result<int, ErrorResponse>> GetCountCompletedTaskAsync(int idSession, CancellationToken ct);
        Task<Session> GetSessionAsync(ISpecification<Session> spec, CancellationToken ct);

        //------------ DATABASE ------------
        Task<PlayerSession> GetVinnerAsync(int idSession, CancellationToken ct);
        Task<Result<Unit, ErrorResponse>> AddSessionInDbAsync(Session session, CancellationToken ct, bool commit = true);
        Task<Session> GetSessionInDbAsync(int id, CancellationToken ct);
        Task<List<Player>> GetListPlayerFromSessionAsync(int idSession, CancellationToken ct);
        Task<int> GetPlayerCountInSessionAsync(int idSession, CancellationToken ct);
        Task<List<Session>> GetListSessionAsync(IFilter<Session>? filter, CancellationToken ct);
        Task DeleteExpiredSessionsInDbAsync(DateTime dateTime, CancellationToken ct, bool commit = true);
        Task FinishExpiredSessionsInDbAsync(DateTime dateTime, CancellationToken ct, bool commit = true);
    }
}
