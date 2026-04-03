
using Ardalis.Specification;
using CodeBattleArena.Application.Features.Players.Filters;

namespace CodeBattleArena.Application.Features.Players.Specifications
{
    public class PlayersListSpec : PlayerBaseSpec
    {
        public PlayersListSpec(PlayerFilter filter) : base()
        {
            Query.AsNoTracking();
            ApplyFilter(filter);
        }

        private void ApplyFilter(PlayerFilter filter)
        {
            Query.Where(x => x.Profile.Name.ToLower().Contains(filter.UserName.ToLower()), !string.IsNullOrEmpty(filter.UserName));

            Query.Where(x => x.Stats.Level >= filter.Level, filter.Level.HasValue);

            Query.Skip((filter.PageNumber - 1) * filter.PageSize)
                     .Take(filter.PageSize);
        }
    }
}
