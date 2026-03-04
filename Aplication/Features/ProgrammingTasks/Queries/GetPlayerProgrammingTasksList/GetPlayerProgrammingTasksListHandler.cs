
using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.ProgrammingTasks.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.ProgrammingTasks;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Queries.GetPlayerProgrammingTasksList
{
    public class GetPlayerProgrammingTasksListHandler : IRequestHandler<GetPlayerProgrammingTasksListQuery, Result<List<ProgrammingTaskDto>>>
    {
        private readonly IRepository<ProgrammingTask> _taskRepository;
        private readonly IMapper _mapper;
        public GetPlayerProgrammingTasksListHandler(IRepository<ProgrammingTask> taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }
        public async Task<Result<List<ProgrammingTaskDto>>> Handle(GetPlayerProgrammingTasksListQuery request, CancellationToken cancellationToken)
        {
            var spec = new ProgrammingTasksListSpec(request.PlayerId);
            var tasks = await _taskRepository.GetBySpecAsync(spec, cancellationToken);
            return Result<List<ProgrammingTaskDto>>.Success(_mapper.Map<List<ProgrammingTaskDto>>(tasks));
        }
    }
}
