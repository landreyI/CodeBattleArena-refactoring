
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Queries.GetProgrammingTask
{
    public record GetProgrammingTaskQuery(Guid Id) : IRequest<Result<ProgrammingTaskDto>>, ICachableRequest
    {
        public string CacheKey => CacheKeys.Tasks.Details(Id);
        public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
    }
}
