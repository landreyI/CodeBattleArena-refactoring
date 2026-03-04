
using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.ProgrammingTasks.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.ProgrammingTasks;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Queries.GetProgrammingTask
{
    public class GetProgrammingTaskHandler : IRequestHandler<GetProgrammingTaskQuery, Result<ProgrammingTaskDto>>
    {
        private readonly IRepository<ProgrammingTask> _programmingTaskRepository;
        private readonly IMapper _mapper;

        public GetProgrammingTaskHandler(IRepository<ProgrammingTask> programmingTaskRepository, IMapper mapper)
        {
            _programmingTaskRepository = programmingTaskRepository;
            _mapper = mapper;
        }
        public async Task<Result<ProgrammingTaskDto>> Handle(GetProgrammingTaskQuery request, CancellationToken cancellationToken)
        {
            var spec = new ProgrammingTaskReadOnlySpec(request.Id);
            var programmingTask = await _programmingTaskRepository.GetBySpecAsync(spec, cancellationToken);

            return Result<ProgrammingTaskDto>.Success(_mapper.Map<ProgrammingTaskDto>(programmingTask));
        }
    }
}
