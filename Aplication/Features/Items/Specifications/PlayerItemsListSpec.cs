
using Ardalis.Specification;
using CodeBattleArena.Application.Features.Items.Filters;
using CodeBattleArena.Domain.Enums;

namespace CodeBattleArena.Application.Features.Items.Specifications
{
    public class PlayerItemsListSpec : PlayerItemBaseSpec
    {
        // Конструктор для списка конкретного игрока
        public PlayerItemsListSpec(Guid playerId, ItemFilter? filter = default) : base()
        {
            Query.Where(pi => pi.PlayerId == playerId)
                 .AsNoTracking();

            AddCommonIncludes();

            if(filter != null)
            ApplyFilter(filter);
        }

        private void ApplyFilter(ItemFilter filter)
        {

            if (!string.IsNullOrWhiteSpace(filter.Type) &&
                Enum.TryParse<TypeItem>(filter.Type, true, out var stateEnum))
            {
                Query.Where(x => x.Item.Type == stateEnum);
            }
            Query.Where(x => x.Item.PriceCoin == filter.Coin, filter.Coin.HasValue);

            if (filter.IsCoinDescending.HasValue && !filter.IsCoinDescending.Value)
                Query.OrderBy(x => x.Item.PriceCoin);
            else
                Query.OrderByDescending(x => x.Item.PriceCoin);

            Query.Skip((filter.PageNumber - 1) * filter.PageSize)
                     .Take(filter.PageSize);
        }
    }
}
