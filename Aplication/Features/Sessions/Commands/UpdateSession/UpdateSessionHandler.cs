
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Sessions.Interfaces;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.ProgrammingTasks;
using CodeBattleArena.Domain.TaskLanguages;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Commands.UpdateSession
{
    public class UpdateSessionHandler : IRequestHandler<UpdateSessionCommand, Result<bool>>
    {
        private readonly IRepository<ProgrammingTask> _taskRepository;
        private readonly IRepository<TaskLanguage> _taskLanguageRepository;
        private readonly ISessionAccessService _accessService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSessionHandler(
            IRepository<ProgrammingTask> taskRepository,
            IRepository<TaskLanguage> taskLanguageRepository,
            ISessionAccessService accessService,
            IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _taskLanguageRepository = taskLanguageRepository;
            _accessService = accessService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
        {
            var sessionResult = await _accessService.GetSessionForUpdateAsync(request.Id, cancellationToken);

            if (sessionResult.IsFailure)
                return Result<bool>.Failure(sessionResult.Error);

            var session = sessionResult.Value;

            // 3. БИЗНЕС-ЛОГИКА: Проверка совместимости Задачи и Языка
            if (request.TaskId.HasValue)
            {
                var taskExists = await _taskRepository.AnyAsync(t => t.Id == request.TaskId.Value, cancellationToken);
                if (!taskExists)
                    return Result<bool>.Failure(new Error("task.not_found", "Selected task not found", 404));

                // Проверяем, поддерживает ли задача выбранный язык (или текущий язык сессии)
                var targetLangId = request.ProgrammingLangId ?? session.ProgrammingLangId;

                var supportsLang = await _taskLanguageRepository.AnyAsync(tl =>
                    tl.ProgrammingTaskId == request.TaskId.Value &&
                    tl.ProgrammingLangId == targetLangId, cancellationToken);

                if (!supportsLang)
                    return Result<bool>.Failure(new Error("task.lang_mismatch", "The selected task does not support the chosen language"));
            }

            var resultUpdate = session.Update(
                request.Name, 
                request.ProgrammingLangId, 
                request.State, 
                request.MaxPeople, 
                request.Password, 
                request.TimePlay, 
                request.TaskId);

            if (resultUpdate.IsFailure)
                return Result<bool>.Failure(resultUpdate.Error);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
