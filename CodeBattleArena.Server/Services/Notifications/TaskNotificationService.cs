using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Hubs;
using CodeBattleArena.Server.Services.Notifications.INotifications;
using Microsoft.AspNetCore.SignalR;

namespace CodeBattleArena.Server.Services.Notifications
{
    public class TaskNotificationService : ITaskNotificationService
    {
        private readonly IHubContext<TaskHub> _hubContext;

        public TaskNotificationService(IHubContext<TaskHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task NotifyTaskAddAsync(TaskProgrammingDto taskDto)
        {
            await _hubContext.Clients.All.SendAsync("TaskAdding", taskDto);
        }
        public async Task NotifyTaskDeletedGroupAsync(int id)
        {
            await _hubContext.Clients.Group($"Task-{id}")
                .SendAsync("TaskDeleting", id);
        }
        public async Task NotifyTaskDeletedAllAsync(int id)
        {
            await _hubContext.Clients.All.SendAsync("TasksListDeleting", id);
        }
        public async Task NotifyTaskUpdatedGroupAsync(TaskProgrammingDto taskDto)
        {
            await _hubContext.Clients.Group($"Task-{taskDto.IdTaskProgramming}")
                .SendAsync("TaskUpdated", taskDto);
        }
        public async Task NotifyTaskUpdatedAllAsync(TaskProgrammingDto taskDto)
        {
            await _hubContext.Clients.All.SendAsync("TasksListUpdated", taskDto);
        }
    }
}
