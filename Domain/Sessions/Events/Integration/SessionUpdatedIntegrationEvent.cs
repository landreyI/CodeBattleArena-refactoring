using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.Sessions.Events.Integration
{
    public record SessionUpdatedIntegrationEvent(Session Session) : IIntegrationEvent;
}
