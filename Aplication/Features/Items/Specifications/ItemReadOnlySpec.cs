
using Ardalis.Specification;

namespace CodeBattleArena.Application.Features.Items.Specifications
{
    public class ItemReadOnlySpec : ItemBaseSpec
    {
        public ItemReadOnlySpec(Guid itemId) : base(itemId)
        {
            Query.AsNoTracking();
        }
    }
}
