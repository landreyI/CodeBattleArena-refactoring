
using CodeBattleArena.Domain.Enums;

namespace CodeBattleArena.Domain.Sessions.Value_Objects
{
    public record SessionAccess
    (
        SessionState Type,
        int MaxPeople,
        string? Password = default
    );
}
