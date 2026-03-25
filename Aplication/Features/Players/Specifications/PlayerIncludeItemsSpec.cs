
using Ardalis.Specification;

namespace CodeBattleArena.Application.Features.Players.Specifications
{
    public class PlayerIncludeItemsSpec : PlayerBaseSpec
    {
        public PlayerIncludeItemsSpec(Guid id) : base(id)
        {
            Query.Include(p => p.PlayerItems)
                 .AsNoTracking();
        }
    }
}
