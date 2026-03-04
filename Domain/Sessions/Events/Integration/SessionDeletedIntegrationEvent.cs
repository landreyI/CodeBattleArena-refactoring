using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.Sessions.Events.Integration
{
    public record SessionDeletedIntegrationEvent(Guid Id) : IIntegrationEvent;
}
