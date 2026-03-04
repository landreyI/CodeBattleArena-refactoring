using Ardalis.Specification;
using CodeBattleArena.Domain.PlayerSessions;

namespace CodeBattleArena.Application.Features.Sessions.Specifications
{
    public class BestResultSpec : Specification<PlayerSession>, ISingleResultSpecification<PlayerSession>
    {
        public BestResultSpec(Guid sessionId)
        {
            Query.Where(ps => ps.SessionId == sessionId &&
                             ps.PlayerId == ps.Session.WinnerId);

            Query.AsNoTracking()
                 .Include(ps => ps.Player)
                 .Include(ps => ps.Session);
        }
    }
}