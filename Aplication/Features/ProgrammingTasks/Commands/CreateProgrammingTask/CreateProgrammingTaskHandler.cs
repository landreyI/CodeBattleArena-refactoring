
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Sessions.Commands.CreateSession;
using CodeBattleArena.Application.Features.Sessions.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.ProgrammingTasks;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Commands.CreateProgrammingTask
{
    public class CreateProgrammingTaskHandler : IRequestHandler<CreateProgrammingTaskCommand, Result<Guid>>
    {
        private readonly IRepository<ProgrammingTask> _taskRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;
        public CreateProgrammingTaskHandler(IRepository<ProgrammingTask> taskRepository, IIdentityService identityService, IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateProgrammingTaskCommand request, CancellationToken cancellationToken)
        {
            var currentPlayerId = _identityService.CurrentPlayerId();
            if (!currentPlayerId.HasValue)
                return Result<Guid>.Failure(new Error("auth.unauthorized", "User not found in context", 401));

            var resultTask = ProgrammingTask.Create(
                request.Name,
                request.Description,
                request.Difficulty,
                currentPlayerId.Value,
                false);

            if (resultTask.IsFailure)
                return Result<Guid>.Failure(resultTask.Error);

            var task = resultTask.Value;
            var resultAddTestCases = task.SyncTestCases(request.TestCases);
            if(resultAddTestCases.IsFailure)
                return Result<Guid>.Failure(resultAddTestCases.Error);

            var resultAddLeanguages = task
                .SyncTaskLanguages(request.TaskLanguages);
            if (resultAddLeanguages.IsFailure)
                return Result<Guid>.Failure(resultAddLeanguages.Error);

            await _taskRepository.AddAsync(resultTask.Value, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid>.Success(resultTask.Value.Id);
        }
    }
}
