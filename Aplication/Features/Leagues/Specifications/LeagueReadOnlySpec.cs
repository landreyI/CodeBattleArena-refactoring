
using Ardalis.Specification;

namespace CodeBattleArena.Application.Features.Leagues.Specifications
{
    public class LeagueReadOnlySpec : LeagueBaseSpec
    {
        public LeagueReadOnlySpec(Guid leagueId) : base(leagueId)
        {
            Query.AsNoTracking();
        }
    }
}
