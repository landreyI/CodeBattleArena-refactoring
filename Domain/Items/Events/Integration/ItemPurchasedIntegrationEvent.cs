using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Players;

namespace CodeBattleArena.Domain.Items.Events.Integration
{
    public record ItemPurchasedIntegrationEvent(Player player, Item item) : IIntegrationEvent;
}
