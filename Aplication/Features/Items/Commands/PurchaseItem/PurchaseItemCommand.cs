
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Commands.PurchaseItem
{
    public record PurchaseItemCommand(Guid ItemId, Guid? PlayerId) : IRequest<Result<bool>>, ICacheInvalidator
    {
        public string[] CacheKeys => [Common.CacheKeys.Items.Details(ItemId)];
        public string[] Tags => [Common.CacheKeys.Items.AllTag];
    }
}
