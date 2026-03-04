namespace CodeBattleArena.Application.Features.Auth.Models
{
    public record UserAuthDto(
    string Id,
    string UserName,
    string? PhotoUrl,
    int? Coin,
    int? Experience,
    IList<string> Roles);
}
