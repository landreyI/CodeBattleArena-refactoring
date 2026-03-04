using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.Sessions.Events.Internal
{
    public record GameFinishedInternalEvent(Session Session) : IInternalEvent;
}
