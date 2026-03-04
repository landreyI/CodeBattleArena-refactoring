
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Queries.GetProgrammingTask
{
    public record GetProgrammingTaskQuery(Guid Id) : IRequest<Result<ProgrammingTaskDto>>;
}
