using Ardalis.Specification;
using CodeBattleArena.Domain.PlayerSessions;

namespace CodeBattleArena.Application.Features.Sessions.Specifications
{
    public class PlayerSessionIncludesSpec : Specification<PlayerSession>, ISingleResultSpecification<PlayerSession>
    {
        // все игроки конкретной сессии
        public PlayerSessionIncludesSpec(Guid sessionId)
        {
            Query.Where(s => s.SessionId == sessionId);

            AddCommonIncludes();
            Query.AsNoTracking();
        }

        // конкретный игрок в конкретной сессии
        public PlayerSessionIncludesSpec(Guid sessionId, Guid playerId)
        {
            Query.Where(s => s.SessionId == sessionId)
                 .Where(s => s.PlayerId == playerId);

            AddCommonIncludes();
            Query.AsNoTracking();
        }

        private void AddCommonIncludes()
        {
            Query.Include(s => s.Player);

            Query.Include(s => s.Session)
                 .ThenInclude(sess => sess.ProgrammingLang);
        }
    }
}