using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.Items.Events.Integration
{
    public record ItemDeletedIntegrationEvent(Guid Id) : IIntegrationEvent;
}
