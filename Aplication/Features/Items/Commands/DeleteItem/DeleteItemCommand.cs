
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Commands.DeleteItem
{
    public record DeleteItemCommand(Guid Id) : IRequest<Result<bool>>, ICacheInvalidator, IStaffRequest
    {
        // Удаляем только одну конкретную карточку по ключу
        public string[] CacheKeys => [Common.CacheKeys.Items.Details(Id)];

        // Удаляем ВСЕ списки, потому что состав изменился
        public string[] Tags => [Common.CacheKeys.Items.ListTag];
    }
}
