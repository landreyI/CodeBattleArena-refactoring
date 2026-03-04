
using Ardalis.Specification;
using CodeBattleArena.Application.Features.Sessions.Filters;
using CodeBattleArena.Domain.Enums;

namespace CodeBattleArena.Application.Features.Sessions.Specifications
{
    public class SessionsListSpec : SessionBaseSpec
    {
        // сессии конкретного игрока
        public SessionsListSpec(Guid playerId) : base()
        {
            AddCommonIncludes();
            Query.Where(s => s.PlayerSessions.Any(p => p.PlayerId == playerId))
                 .AsNoTracking()
                 .OrderByDescending(x => x.DateCreating);
        }

        public SessionsListSpec(SessionFilter filter) : base()
        {
            AddCommonIncludes();

            Query.AsNoTracking()
                 .OrderByDescending(x => x.DateCreating);

            Query.Where(x => x.ProgrammingLangId == filter.IdLang, filter.IdLang.HasValue);

            Query.Where(x => x.Access.MaxPeople <= filter.MaxPeople, filter.MaxPeople.HasValue);

            if (!string.IsNullOrWhiteSpace(filter.SessionState) &&
                Enum.TryParse<SessionState>(filter.SessionState, true, out var stateEnum))
            {
                Query.Where(x => x.Access.Type == stateEnum);
            }

            if (!string.IsNullOrWhiteSpace(filter.Status) &&
                Enum.TryParse<GameStatus>(filter.Status, true, out var statusEnum))
            {
                Query.Where(x => x.Status == statusEnum);
            }
        }
    }
}
