using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.PlayerItems.Events.Integration
{
    public record ItemReceivedIntegrationEvent(PlayerItem PlayerItem) : IIntegrationEvent;
}
