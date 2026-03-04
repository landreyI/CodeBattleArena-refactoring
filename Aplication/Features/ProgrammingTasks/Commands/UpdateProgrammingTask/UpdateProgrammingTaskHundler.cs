
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.ProgrammingTasks.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.ProgrammingTasks;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Commands.UpdateProgrammingTask
{
    public class UpdateProgrammingTaskHundler : IRequestHandler<UpdateProgrammingTaskCommand, Result<bool>>
    {
        private readonly IRepository<ProgrammingTask> _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateProgrammingTaskHundler(IRepository<ProgrammingTask> taskRepository, IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateProgrammingTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetBySpecAsync(new ProgrammingTaskForUpdateSpec(request.Id));
            if (task == null)
                return Result<bool>.Failure(new Error("task.not_found", "Selected task not found", 404));

            var resultUpdate = task.Update(
                request.Name,
                request.Description,
                request.Difficulty);

            if (resultUpdate.IsFailure)
                return Result<bool>.Failure(resultUpdate.Error);

            var resultSyncTestCases = task.SyncTestCases(request.TestCases);
            if (resultSyncTestCases.IsFailure)
                return Result<bool>.Failure(resultSyncTestCases.Error);

            var resultSyncTaskLanguages = task.SyncTaskLanguages(request.TaskLanguages);
            if (resultSyncTaskLanguages.IsFailure)
                return Result<bool>.Failure(resultSyncTaskLanguages.Error);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
