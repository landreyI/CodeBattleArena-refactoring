
using Ardalis.Specification;

namespace CodeBattleArena.Application.Features.Players.Specifications
{
    public class PlayerForUpdateSpec : PlayerBaseSpec
    {
        public PlayerForUpdateSpec(Guid id) : base(id)
        {
            Query.Include(p => p.League);
        }
    }
}