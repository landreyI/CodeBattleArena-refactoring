using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.Items.Events.Integration
{
    public record ItemCreatedIntegrationEvent(Item Item) : IIntegrationEvent;
}
