using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Domain.ProgrammingTasks.Events.Integration;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.EventHandlers.Integration
{
    internal class ProgrammingTaskDeletedEventHandler 
        : INotificationHandler<DomainEventNotification<ProgrammingTaskDeletedIntegrationEvent>>
    {
        private readonly ITaskNotificationService _taskNotificationService;

        public ProgrammingTaskDeletedEventHandler(ITaskNotificationService taskNotificationService)
        {   
            _taskNotificationService = taskNotificationService;
        }

        public async Task Handle(DomainEventNotification<ProgrammingTaskDeletedIntegrationEvent> notification, CancellationToken ct)
        {
            await _taskNotificationService.NotifyTaskDeletedAllAsync(notification.DomainEvent.Id);
            await _taskNotificationService.NotifyTaskDeletedGroupAsync(notification.DomainEvent.Id);
        }
    }
}
