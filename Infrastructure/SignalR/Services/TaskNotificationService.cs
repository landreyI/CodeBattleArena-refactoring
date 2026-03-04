using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.TaskLanguages;
using CodeBattleArena.Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CodeBattleArena.Infrastructure.SignalR.Services
{
    public class TaskNotificationService : ITaskNotificationService
    {
        private readonly IHubContext<MainHub> _hubContext;

        public TaskNotificationService(IHubContext<MainHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task NotifyTaskAddAsync(ProgrammingTaskDto taskDto)
        {
            await _hubContext.Clients.All.SendAsync("TaskAdding", taskDto);
        }
        public async Task NotifyTaskDeletedGroupAsync(Guid id)
        {
            await _hubContext.Clients.Group($"Task-{id}")
                .SendAsync("TaskDeleting", id);
        }
        public async Task NotifyTaskDeletedAllAsync(Guid id)
        {
            await _hubContext.Clients.All.SendAsync("TasksListDeleting", id);
        }
        public async Task NotifyTaskUpdatedGroupAsync(ProgrammingTaskDto taskDto)
        {
            await _hubContext.Clients.Group($"Task-{taskDto.Id}")
                .SendAsync("TaskUpdated", taskDto);
        }
        public async Task NotifyTaskUpdatedAllAsync(ProgrammingTaskDto taskDto)
        {
            await _hubContext.Clients.All.SendAsync("TasksListUpdated", taskDto);
        }

        // AI Метаданные готовы (или упал)
        public async Task NotifyMetadataReadyAsync(Guid jobId, ProgrammingTaskDto? taskDto = default, string? error = default)
        {
            await _hubContext.Clients.Group($"Job-{jobId}")
                .SendAsync("MetadataReady", new { 
                    taskDto,
                    error
                });
        }

        // AI Конкретный язык готов (или упал)
        public async Task NotifyLanguageStatusAsync(Guid jobId, Guid langId, string langName, bool success, string? error = default)
        {
            await _hubContext.Clients.Group($"Job-{jobId}")
                .SendAsync("LanguageStatus", new
                {
                    LangId = langId,
                    LangName = langName,
                    Success = success,
                    Error = error
                });
        }
    }
}
