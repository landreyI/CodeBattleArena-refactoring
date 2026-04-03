
using Ardalis.Specification;

namespace CodeBattleArena.Application.Features.Players.Specifications
{
    public class PlayerReadOnlySpec : PlayerBaseSpec
    {
        public PlayerReadOnlySpec(Guid id) : base(id)
        {
            Query.Include(p => p.League)
                 .AsNoTracking();
        }
    }
}
