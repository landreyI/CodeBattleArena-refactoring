
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Players.Commands.UpdatePlayer
{
    public record UpdatePlayerCommand(
        Guid Id,
        string? Name,
        string? PhotoUrl,
        string? Info) : IRequest<Result<bool>>, ICacheInvalidator
    {

        // Удаляем только одну конкретную карточку по ключу
        public string[] CacheKeys => [Common.CacheKeys.Players.Details(Id)];

        // Удаляем ВСЕ списки, потому что состав изменился
        public string[] Tags => [Common.CacheKeys.Players.ListTag];
    }
}
