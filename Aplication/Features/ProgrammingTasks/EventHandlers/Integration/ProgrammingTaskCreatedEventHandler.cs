using AutoMapper;
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.ProgrammingTasks.Specifications;
using CodeBattleArena.Domain.ProgrammingTasks;
using CodeBattleArena.Domain.ProgrammingTasks.Events.Integration;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.EventHandlers.Integration
{
    public class ProgrammingTaskCreatedEventHandler 
        : INotificationHandler<DomainEventNotification<ProgrammingTaskCreatedIntegrationEvent>>
    {
        private readonly ITaskNotificationService _taskNotificationService;
        private readonly IRepository<ProgrammingTask> _taskRepository;
        private readonly IMapper _mapper;

        public ProgrammingTaskCreatedEventHandler(ITaskNotificationService taskNotificationService, IRepository<ProgrammingTask> taskRepository, IMapper mapper)
        {
            _taskNotificationService = taskNotificationService;
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task Handle(DomainEventNotification<ProgrammingTaskCreatedIntegrationEvent> notification, CancellationToken ct)
        {
            var task = await _taskRepository.GetBySpecAsync(new ProgrammingTaskReadOnlySpec(notification.DomainEvent.Task.Id), ct);
            var dto = _mapper.Map<ProgrammingTaskDto>(task);

            await _taskNotificationService.NotifyTaskAddAsync(dto);
        }
    }
}
