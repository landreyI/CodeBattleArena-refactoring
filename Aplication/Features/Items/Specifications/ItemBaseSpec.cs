
using Ardalis.Specification;
using CodeBattleArena.Domain.Items;

namespace CodeBattleArena.Application.Features.Items.Specifications
{
    public abstract class ItemBaseSpec : Specification<Item>
    {
        protected ItemBaseSpec() { }

        protected ItemBaseSpec(Guid itemId) : this()
        {
            Query.Where(s => s.Id == itemId);
        }
    }
}
