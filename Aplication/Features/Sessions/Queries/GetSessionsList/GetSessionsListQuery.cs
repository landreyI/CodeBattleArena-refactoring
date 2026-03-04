using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Sessions.Filters;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSessionsList
{
    public record GetSessionsListQuery(SessionFilter? Filter) : IRequest<Result<List<SessionDto>>>;
}
