
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Commands.UpdateItem
{
    public record UpdateItemCommand(
        Guid Id,
        string? Name,
        TypeItem? Type,
        int? PriceCoin,
        string? CssClass,
        string? ImageUrl,
        string? Description) : IRequest<Result<bool>>, ICacheInvalidator, IStaffRequest
    {

        // Удаляем только одну конкретную карточку по ключу
        public string[] CacheKeys => [Common.CacheKeys.Items.Details(Id)];

        // Удаляем ВСЕ списки, потому что состав изменился
        public string[] Tags => [Common.CacheKeys.Items.ListTag];
    }
}
