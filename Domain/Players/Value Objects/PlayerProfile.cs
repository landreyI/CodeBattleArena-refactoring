
namespace CodeBattleArena.Domain.Players.Value_Objects
{
    public record PlayerProfile(string Name, string? PhotoUrl = default, string? AdditionalInformation = default);
}
