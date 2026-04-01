

using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Leagues.Commands.UpdateLeague
{
    public record UpdateLeagueCommand(
        Guid Id,
        string? Name,
        string? ImageUrl,
        int? MinWins,
        int? MaxWins
    ) : IRequest<Result<bool>>, ICacheInvalidator, IStaffRequest
    {
        // Удаляем только одну конкретную карточку по ключу
        public string[] CacheKeys => [Common.CacheKeys.Leagues.Details(Id)];

        // Удаляем ВСЕ списки, потому что состав изменился
        public string[] Tags => [Common.CacheKeys.Leagues.ListTag];
    }
}
