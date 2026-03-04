using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Common
{
    public class DomainEventNotification<TEvent> : INotification where TEvent : IDomainEvent
    {
        public TEvent DomainEvent { get; }
        public DomainEventNotification(TEvent domainEvent) => DomainEvent = domainEvent;
    }
}
