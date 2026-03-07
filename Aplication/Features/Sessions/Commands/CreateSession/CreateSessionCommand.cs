using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Commands.CreateSession
{
    public record CreateSessionCommand(
        string Name,
        Guid ProgrammingLangId,
        SessionState State,
        int MaxPeople,
        string? Password,
        int? TimePlay,
        Guid? TaskId) : IRequest<Result<Guid>>, ICacheInvalidator
    {
        public string[] Tags => [Common.CacheKeys.Sessions.ListTag];
    }
}
