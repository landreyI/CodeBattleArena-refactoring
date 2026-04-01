
using Ardalis.Specification;
using CodeBattleArena.Domain.Leagues;

namespace CodeBattleArena.Application.Features.Leagues.Specifications
{
    public class LeagueBaseSpec : Specification<League>
    {
        protected LeagueBaseSpec() { }

        protected LeagueBaseSpec(Guid leagueId) : this()
        {
            Query.Where(s => s.Id == leagueId);
            AddCommonIncludes();
        }

        protected void AddCommonIncludes()
        {
            Query.Include(p => p.Players);
        }
    }
}
