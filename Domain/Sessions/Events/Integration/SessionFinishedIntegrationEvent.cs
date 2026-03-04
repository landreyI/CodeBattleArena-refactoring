using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.Sessions.Events.Integration
{
    public record SessionFinishedIntegrationEvent(Session Session) : IIntegrationEvent;
}
