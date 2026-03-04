using Ardalis.Specification;
using CodeBattleArena.Domain.Sessions;
using CodeBattleArena.Domain.Enums;

namespace CodeBattleArena.Application.Features.Sessions.Specifications
{
    public class ActiveSessionReadOnlySpec : SessionBaseSpec, ISingleResultSpecification<Session>
    {
        public ActiveSessionReadOnlySpec(Guid playerId) : base()
        {
            AddCommonIncludes();
            Query.Where(s => s.PlayerSessions.Any(ps => ps.PlayerId == playerId)
                             && s.Status != GameStatus.Finished);

            Query.AsNoTracking();
        }
    }
}