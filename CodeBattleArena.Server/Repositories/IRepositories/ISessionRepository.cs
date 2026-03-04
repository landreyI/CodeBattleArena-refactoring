using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Specifications;
using CodeBattleArena.Server.Specifications.SessionSpec;

namespace CodeBattleArena.Server.IRepositories
{
    public interface ISessionRepository
    {
        Task StartGameAsync(int idSession, CancellationToken cancellationToken);
        Task FinishGameAsync(int idSession, CancellationToken cancellationToken);
        Task<PlayerSession> GetVinnerAsync(int idSession, CancellationToken cancellationToken);
        Task AddSessionAsync(Session session, CancellationToken cancellationToken);
        Task AddTaskToSession(int idSession, int idTask, CancellationToken cancellationToken);
        Task<Session> GetSessionAsync(ISpecification<Session> spec, CancellationToken cancellationToken);
        Task ChangePasswordSessionAsync(int idSession, string password, CancellationToken cancellationToken);
        Task DelSessionAsync(int id, CancellationToken cancellationToken);
        Task DelTaskToSession(int idSession, CancellationToken cancellationToken);
        Task<List<Player>> GetListPlayerFromSessionAsync(int idSession, CancellationToken cancellationToken);
        Task<int> GetPlayerCountInSessionAsync(int idSession, CancellationToken cancellationToken);
        Task<List<Session>> GetListSessionAsync(ISpecification<Session> spec, CancellationToken cancellationToken);
        Task<List<int>> DeleteExpiredSessionsAsync(DateTime dateTime, CancellationToken cancellationToken);
        Task<List<int>> FinishExpiredSessionsAsync(DateTime dateTime, CancellationToken cancellationToken);
        Task UpdateSession(Session session);

    }
}
