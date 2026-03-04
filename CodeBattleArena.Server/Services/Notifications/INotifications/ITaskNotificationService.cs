using CodeBattleArena.Server.DTO.ModelsDTO;

namespace CodeBattleArena.Server.Services.Notifications.INotifications
{
    public interface ITaskNotificationService
    {
        Task NotifyTaskAddAsync(TaskProgrammingDto taskDto);
        Task NotifyTaskDeletedGroupAsync(int id);
        Task NotifyTaskDeletedAllAsync(int id);
        Task NotifyTaskUpdatedGroupAsync(TaskProgrammingDto taskDto);
        Task NotifyTaskUpdatedAllAsync(TaskProgrammingDto taskDto);
    }
}
