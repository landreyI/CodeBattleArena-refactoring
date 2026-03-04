using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSessionPlayers
{
    public record GetSessionPlayersQuery(Guid Id) : IRequest<Result<List<PlayerSessionDto>>>;
}
