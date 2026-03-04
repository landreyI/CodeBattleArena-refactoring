using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.Sessions.Events.Integration
{
    public record SessionJoinedIntegrationEvent(Guid SessionId, Guid PlayerId) : IIntegrationEvent;
}
