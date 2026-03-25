
using Ardalis.Specification;
using CodeBattleArena.Domain.PlayerItems;

namespace CodeBattleArena.Application.Features.Items.Specifications
{
    public abstract class PlayerItemBaseSpec : Specification<PlayerItem>
    {
        protected PlayerItemBaseSpec() { }

        protected PlayerItemBaseSpec(Guid playerId, Guid itemId) : this()
        {
            Query.Where(pi => pi.PlayerId == playerId && pi.ItemId == itemId);
            AddCommonIncludes();
        }

        protected void AddCommonIncludes()
        {
            Query.Include(p => p.Payer)
                 .Include(i => i.Item)
                 .Include(p => p.Player)
                 .ThenInclude(pi => pi.PlayerItems);
        }
    }
}
