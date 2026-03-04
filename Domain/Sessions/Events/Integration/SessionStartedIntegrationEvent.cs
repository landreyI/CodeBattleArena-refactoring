using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.Sessions.Events.Integration
{
    public record SessionStartedIntegrationEvent(Session Session) : IIntegrationEvent;
}
