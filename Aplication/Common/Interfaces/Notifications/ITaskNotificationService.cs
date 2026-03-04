using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.TaskLanguages;

namespace CodeBattleArena.Application.Common.Interfaces.Notifications
{
    public interface ITaskNotificationService
    {
        Task NotifyTaskAddAsync(ProgrammingTaskDto taskDto);
        Task NotifyTaskDeletedGroupAsync(Guid id);
        Task NotifyTaskDeletedAllAsync(Guid id);
        Task NotifyTaskUpdatedGroupAsync(ProgrammingTaskDto taskDto);
        Task NotifyTaskUpdatedAllAsync(ProgrammingTaskDto taskDto);
        Task NotifyMetadataReadyAsync(Guid jobId, ProgrammingTaskDto? taskDto = default, string? error = default);
        Task NotifyLanguageStatusAsync(Guid jobId, Guid langId, string langName, bool success, string? error = default);

    }
}
