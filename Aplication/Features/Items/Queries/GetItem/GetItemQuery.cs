
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Queries.GetItem
{
    public record GetItemQuery(Guid Id) : IRequest<Result<ItemDto>>, ICachableRequest
    {
        public string CacheKey => CacheKeys.Items.Details(Id);
        public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
    }
}
