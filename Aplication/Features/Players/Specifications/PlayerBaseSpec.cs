
using Ardalis.Specification;
using CodeBattleArena.Domain.Players;

namespace CodeBattleArena.Application.Features.Players.Specifications
{
    public class PlayerBaseSpec : Specification<Player>
    {
        protected PlayerBaseSpec() { }

        protected PlayerBaseSpec(Guid id) : this()
        {
            Query.Where(s => s.Id == id);
        }
    }
}
