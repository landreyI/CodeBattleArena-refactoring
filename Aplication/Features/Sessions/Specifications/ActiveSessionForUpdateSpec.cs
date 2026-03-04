using Ardalis.Specification;
using CodeBattleArena.Domain.Sessions;
using CodeBattleArena.Domain.Enums;

namespace CodeBattleArena.Application.Features.Sessions.Specifications
{
    public class ActiveSessionForUpdateSpec : SessionBaseSpec, ISingleResultSpecification<Session>
    {
        public ActiveSessionForUpdateSpec(Guid playerId) : base()
        {
            AddCommonIncludes();
            Query.Where(s => s.PlayerSessions.Any(ps => ps.PlayerId == playerId)
                             && s.Status != GameStatus.Finished);
        }
    }
}