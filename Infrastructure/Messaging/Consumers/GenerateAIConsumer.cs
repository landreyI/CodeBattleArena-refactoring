using AutoMapper;
using CodeBattleArena.Application.Common.Contracts;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Infrastructure.SignalR.Services;
using MassTransit;
using MediatR;

namespace CodeBattleArena.Infrastructure.Messaging.Consumers
{
    public class GenerateAIConsumer : IConsumer<GenerateAITaskMessage>
    {
        private readonly IMediator _mediator;
        private readonly IAIService _aIService;
        private readonly IMapper _mapper;
        private readonly ITaskNotificationService _taskNotificationService;
        private readonly INotificationService _notificationService;

        public GenerateAIConsumer(IMediator mediator, IAIService aIService, IMapper mapper, ITaskNotificationService taskNotificationService, INotificationService notificationService)
        {
            _mediator = mediator;
            _aIService = aIService;
            _mapper = mapper;
            _taskNotificationService = taskNotificationService;
            _notificationService = notificationService;
        }

        public async Task Consume(ConsumeContext<GenerateAITaskMessage> context)
        {
            var message = context.Message;

            var aiResult = await _aIService.GenerateTaskMetadataAndTestsAsync(message);
            if (aiResult.IsSuccess && aiResult.Value != null)
            {
                foreach (var langId in message.ProgrammingLangIds)
                {
                    await context.Publish(new GenerateAILanguageCodeMessage
                    {
                        TaskId = aiResult.Value.Id,
                        LanguageId = langId,
                        Prompt = message.Prompt,
                        JobId = message.JobId,
                        UserId = message.UserId,
                    });
                }

                var dto = _mapper.Map<ProgrammingTaskDto>(aiResult.Value);
                await _taskNotificationService.NotifyMetadataReadyAsync(message.JobId, dto);
                await _notificationService.SendNotificationAsync(
                    message.UserId,
                    $"AI generated metadata for your programming task: '{aiResult.Value.Name}'",
                    NotificationType.TaskGenerated,
                    aiResult.Value.Id);


                await _taskNotificationService.NotifyTaskUpdatedGroupAsync(dto);
                await _taskNotificationService.NotifyTaskUpdatedAllAsync(dto);
            }
        }
    }
}
