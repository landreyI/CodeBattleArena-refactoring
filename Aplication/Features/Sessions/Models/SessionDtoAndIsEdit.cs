using CodeBattleArena.Application.Common.Models.Dtos;

namespace CodeBattleArena.Application.Features.Sessions.Models
{
    public record SessionDtoAndIsEdit(SessionDto Session, bool IsEdit);
}
