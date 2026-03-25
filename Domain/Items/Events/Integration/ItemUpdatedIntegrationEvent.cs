using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.Items.Events.Integration
{
    public record ItemUpdatedIntegrationEvent(Item Item) : IIntegrationEvent;
}
