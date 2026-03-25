
using Ardalis.Specification;

namespace CodeBattleArena.Application.Features.Items.Specifications
{
    public class PlayerItemReadOnlySpec : PlayerItemBaseSpec
    {
        public PlayerItemReadOnlySpec(Guid playerId, Guid itemId) : base(playerId, itemId)
        {
            Query.AsNoTracking();
        }
    }
}
