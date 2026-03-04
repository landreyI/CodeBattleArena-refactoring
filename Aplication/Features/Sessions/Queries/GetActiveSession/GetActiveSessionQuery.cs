using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetActiveSession
{
    public class GetActiveSessionQuery() : IRequest<Result<SessionDto?>>;
}
