using AutoMapper;
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Sessions.Events.Integration;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.EventHandlers.Integration
{
    public class SessionStartedEventHandler
        : INotificationHandler<DomainEventNotification<SessionStartedIntegrationEvent>>
    {
        private readonly ISessionNotificationService _sessionNotificationService;
        private readonly IMapper _mapper;

        public SessionStartedEventHandler(ISessionNotificationService sessionNotificationService, IMapper mapper)
        {
            _sessionNotificationService = sessionNotificationService;
            _mapper = mapper;
        }

        public async Task Handle(DomainEventNotification<SessionStartedIntegrationEvent> notification, CancellationToken ct)
        {
            await _sessionNotificationService.NotifyStartGameAsync(notification.DomainEvent.Session.Id);

            var dto = _mapper.Map<SessionDto>(notification.DomainEvent.Session);
            await _sessionNotificationService.NotifySessionUpdatedGroupAsync(dto);
            await _sessionNotificationService.NotifySessionUpdatedAllAsync(dto);
        }
    }
}
