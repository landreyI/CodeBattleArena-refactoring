using Ardalis.Specification;

namespace CodeBattleArena.Application.Features.Items.Specifications
{
    public class PlayerActiveItemsListSpec : PlayerItemBaseSpec
    {
        // Конструкторы для списка конкретного игрока
        public PlayerActiveItemsListSpec(Guid playerId)
        {
            Query.Where(pi => pi.PlayerId == playerId && pi.IsEquipped)
              .Include(pi => pi.Item)
              .AsNoTracking();
        }
    }
}
