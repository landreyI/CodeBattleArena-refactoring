using AutoMapper;
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.ProgrammingTasks.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.ProgrammingTasks;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Queries.GetProgrammingTasksList
{
    public class GetProgrammingTasksListHandler
    : IRequestHandler<GetProgrammingTasksListQuery, Result<PaginatedResult<ProgrammingTaskDto>>>
    {
        private readonly IRepository<ProgrammingTask> _taskRepository;
        private readonly IMapper _mapper;
        public GetProgrammingTasksListHandler(IRepository<ProgrammingTask> taskRepository, IMapper mapper) 
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }
        public async Task<Result<PaginatedResult<ProgrammingTaskDto>>> Handle(
        GetProgrammingTasksListQuery request, CancellationToken ct)
        {
            var spec = new ProgrammingTasksListSpec(request.Filter);

            var tasks = await _taskRepository.GetListBySpecAsync(spec, ct);

            var totalCount = await _taskRepository.CountAsync(spec, ct);

            var dtos = _mapper.Map<List<ProgrammingTaskDto>>(tasks);

            var result = new PaginatedResult<ProgrammingTaskDto>(
                dtos,
                totalCount,
                request.Filter?.PageNumber ?? 1,
                request.Filter?.PageSize ?? 15);

            return Result<PaginatedResult<ProgrammingTaskDto>>.Success(result);
        }
    }
}
