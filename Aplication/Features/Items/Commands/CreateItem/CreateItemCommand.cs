
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Commands.CreateItem
{
    public record CreateItemCommand(
        string Name,
        TypeItem Type,
        int? PriceCoin,
        string? CssClass,
        string? ImageUrl,
        string? Description

    ) : IRequest<Result<Guid>>, ICacheInvalidator
    {
        // Удаляем ВСЕ списки, потому что состав изменился
        public string[] Tags => [Common.CacheKeys.Items.ListTag];
    }
}
