
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Commands.EquipItem
{
    public record EquipItemCommand(Guid ItemId) : IRequest<Result<bool>>;
}
