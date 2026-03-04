using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Commands.UpdateSession
{
    public record UpdateSessionCommand(
        Guid Id,
        string? Name = null,
        Guid? ProgrammingLangId = null,
        SessionState? State = null,
        int? MaxPeople = null,
        string? Password = null,
        int? TimePlay = null,
        Guid? TaskId = null
    ) : IRequest<Result<bool>>;
}
