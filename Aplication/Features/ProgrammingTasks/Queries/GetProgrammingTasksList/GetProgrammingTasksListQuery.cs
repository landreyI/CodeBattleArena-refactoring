
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.ProgrammingTasks.Filters;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Queries.GetProgrammingTasksList
{
    public record GetProgrammingTasksListQuery(ProgrammingTaskFilter? Filter) : IRequest<Result<List<ProgrammingTaskDto>>>;
}
