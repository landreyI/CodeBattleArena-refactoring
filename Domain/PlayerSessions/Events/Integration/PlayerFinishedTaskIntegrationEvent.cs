using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.PlayerSessions.Events.Integration
{
    public record UpdatePlayerSessionIntegrationEvent(PlayerSession PlayerSession) : IIntegrationEvent;
}
