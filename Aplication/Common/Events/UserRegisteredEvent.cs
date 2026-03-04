using MediatR;

namespace CodeBattleArena.Application.Common.Events
{
    public record UserRegisteredEvent(Guid UserId, string Email, string Name) : INotification;
}
