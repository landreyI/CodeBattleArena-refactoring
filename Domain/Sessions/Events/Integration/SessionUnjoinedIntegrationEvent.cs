using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.Sessions.Events.Integration
{
    public record SessionUnjoinedIntegrationEvent(Guid SessionId, Guid PlayerId) : IIntegrationEvent;
}
