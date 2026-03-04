using CodeBattleArena.Server.Models;
using System.Linq.Expressions;

namespace CodeBattleArena.Server.Specifications.PlayerSessionSpec
{
    public class PlayerSessionByIdSpec : Specification<PlayerSession>
    {
        public PlayerSessionByIdSpec(int? idSession = null, string? idPlayer = null)
        {
            AddInclude(ps => ps.Session);
            AddInclude(ps => ps.Session.PlayerSessions);
            var criteria = new List<Expression<Func<PlayerSession, bool>>>();

            if (idSession.HasValue)
                criteria.Add(ps => ps.IdSession == idSession.Value);

            if (!string.IsNullOrEmpty(idPlayer))
                criteria.Add(ps => ps.IdPlayer == idPlayer);

            if (criteria.Any())
            {
                var parameter = Expression.Parameter(typeof(PlayerSession), "ps");
                var combinedBody = criteria
                    .Select(c => new ParameterRebinder(c.Parameters[0], parameter).Visit(c.Body))
                    .Aggregate(Expression.AndAlso);
                Criteria = Expression.Lambda<Func<PlayerSession, bool>>(combinedBody, parameter);
            }
            else
            {
                Criteria = ps => true;
            }
        }
    }
}
