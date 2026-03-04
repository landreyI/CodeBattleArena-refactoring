
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Sessions;

namespace CodeBattleArena.Application.Features.Sessions.Interfaces
{
    public interface ISessionAccessService
    {
        // Возвращает сессию только если пользователь авторизован и имеет права
        Task<Result<Session>> GetSessionForUpdateAsync(Guid sessionId, CancellationToken ct);
    }
}
