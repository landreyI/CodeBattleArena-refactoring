
using AutoMapper;
using CodeBattleArena.Application.Common.Contracts;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Application.Common.Models.Dtos;
using MassTransit;
using MediatR;

namespace CodeBattleArena.Infrastructure.Messaging.Consumers
{
    public class GenerateAILanguageCodeConsumer : IConsumer<GenerateAILanguageCodeMessage>
    {
        private readonly IMediator _mediator;
        private readonly IAIService _aIService;
        private readonly ITaskNotificationService _taskNotificationService;
        private readonly IMapper _mapper;

        public GenerateAILanguageCodeConsumer(IMediator mediator, IAIService aIService, ITaskNotificationService taskNotificationService, IMapper mapper)
        {
            _mediator = mediator;
            _aIService = aIService;
            _taskNotificationService = taskNotificationService;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<GenerateAILanguageCodeMessage> context)
        {
            var message = context.Message;
            
            var aiResponse = await _aIService.GenerateLanguageCodeAsync(message);
            if (aiResponse.IsSuccess && aiResponse.Value.Item1 != null && aiResponse.Value.Item2 != null )
                return; // добавить signalR

            await _taskNotificationService.NotifyLanguageStatusAsync(
                message.JobId,
                message.LanguageId,
                aiResponse.Value.Item2.ProgrammingLang.Name,
                aiResponse.IsSuccess,
                aiResponse.IsFailure ? aiResponse.Error.Message : null
            );
        }
    }
}
