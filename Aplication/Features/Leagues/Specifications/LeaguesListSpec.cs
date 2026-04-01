
using Ardalis.Specification;

namespace CodeBattleArena.Application.Features.Leagues.Specifications
{
    public class LeaguesListSpec : LeagueBaseSpec
    {
        public LeaguesListSpec() : base()
        {
            AddCommonIncludes();
            Query.AsNoTracking();
        }
    }
}
