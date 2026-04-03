using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Items;

namespace CodeBattleArena.Domain.Players.Events.Integration
{
    public record PlayerItemEquippedIntegrationEvent(Player Player, Item Item) : IIntegrationEvent;
}
