using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Queries.GetPlayerProgrammingTasksList
{
    public record GetPlayerProgrammingTasksListQuery(Guid PlayerId) : IRequest<Result<List<ProgrammingTaskDto>>>;
}
