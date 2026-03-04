using AutoMapper;
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Sessions.Events.Integration;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.EventHandlers.Integration
{
    public class SessionUpdatedEventHandler
    : INotificationHandler<DomainEventNotification<SessionUpdatedIntegrationEvent>>
    {
        private readonly ISessionNotificationService _sessionNotificationService;
        private readonly IMapper _mapper;

        public SessionUpdatedEventHandler(ISessionNotificationService sessionNotificationService, IMapper mapper)
        {
            _sessionNotificationService = sessionNotificationService;
            _mapper = mapper;
        }

        public async Task Handle(DomainEventNotification<SessionUpdatedIntegrationEvent> notification, CancellationToken ct)
        {
            var dto = _mapper.Map<SessionDto>(notification.DomainEvent.Session);

            await _sessionNotificationService.NotifySessionUpdatedGroupAsync(dto);
            await _sessionNotificationService.NotifySessionUpdatedAllAsync(dto);
        }
    }
}
