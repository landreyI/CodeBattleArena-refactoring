using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.PlayerSessions.Events.Integration
{
    public record PlayerFinishedTaskIntegrationEvent(Guid SessionId, Guid PlayerId) : IIntegrationEvent;
}
