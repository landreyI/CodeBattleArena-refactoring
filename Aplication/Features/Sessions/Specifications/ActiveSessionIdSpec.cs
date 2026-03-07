using Ardalis.Specification;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.Sessions;

namespace CodeBattleArena.Application.Features.Sessions.Specifications
{
    public class ActiveSessionIdSpec : Specification<Session>, ISingleResultSpecification<Session>
    {
        public ActiveSessionIdSpec(Guid playerId) : base()
        {
            Query.Where(s => s.PlayerSessions.Any(ps => ps.PlayerId == playerId)
                             && s.Status != GameStatus.Finished);
        }
    }
}
