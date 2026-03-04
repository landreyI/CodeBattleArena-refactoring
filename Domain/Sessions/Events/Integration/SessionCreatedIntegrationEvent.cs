using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.Sessions.Events.Integration
{
    public record SessionCreatedIntegrationEvent(Session Session) : IIntegrationEvent;
}
