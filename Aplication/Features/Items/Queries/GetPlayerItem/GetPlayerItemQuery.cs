
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Queries.GetPlayerItem
{
    public record GetPlayerItemQuery(Guid PlayerId, Guid ItemId) : IRequest<Result<PlayerItemDto>>;
}
