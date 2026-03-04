
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.ProgrammingTasks.Specifications;
using CodeBattleArena.Application.Features.Sessions.Commands.DeleteSession;
using CodeBattleArena.Application.Features.Sessions.Interfaces;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.ProgrammingTasks;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Commands.DeleteProgrammingTask
{
    public class DeleteProgrammingTaskHandler : IRequestHandler<DeleteProgrammingTaskCommand, Result<bool>>
    {
        private readonly IRepository<ProgrammingTask> _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteProgrammingTaskHandler(
            IRepository<ProgrammingTask> taskRepository,
            IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteProgrammingTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetBySpecAsync(new ProgrammingTaskForUpdateSpec(request.Id));
            if (task == null)
                return Result<bool>.Failure(new Error("task.not_found", "Selected task not found", 404));

            task.Delete();
            _taskRepository.Remove(task);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
