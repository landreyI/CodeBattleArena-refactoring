
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Leagues.Commands.CreateLeague
{
    public record CreateLeagueCommand(
        string Name,
        string? ImageUrl,
        int MinWins,
        int? MaxWins
    ) : IRequest<Result<Guid>>, ICacheInvalidator, IStaffRequest
    {
        // Удаляем ВСЕ списки, потому что состав изменился
        public string[] Tags => [Common.CacheKeys.Leagues.ListTag];
    }
}
