
using Ardalis.Specification;
using CodeBattleArena.Application.Features.Items.Filters;
using CodeBattleArena.Domain.Enums;

namespace CodeBattleArena.Application.Features.Items.Specifications
{
    public class ItemsListSpec : ItemBaseSpec
    {
        // Конструктор для общего списка
        public ItemsListSpec(ItemFilter filter) : base()
        {
            Query.AsNoTracking();
            ApplyFilter(filter);
        }

        private void ApplyFilter(ItemFilter filter)
        {

            if (!string.IsNullOrWhiteSpace(filter.Type) &&
                Enum.TryParse<TypeItem>(filter.Type, true, out var stateEnum))
            {
                Query.Where(x => x.Type == stateEnum);
            }
            Query.Where(x => x.PriceCoin == filter.Coin, filter.Coin.HasValue);

            if(filter.IsCoinDescending.HasValue && !filter.IsCoinDescending.Value)
                Query.OrderBy(x => x.PriceCoin);
            else
                Query.OrderByDescending(x => x.PriceCoin);

            Query.Skip((filter.PageNumber - 1) * filter.PageSize)
                     .Take(filter.PageSize);
        }
    }
}
