
using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Domain.Common
{
    public abstract class BaseEntity<TId>
    {
        [Key]
        public TId Id { get; protected set; }


        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            // Проверяем, есть ли уже событие такого типа
            var existingEvent = _domainEvents.FirstOrDefault(e => e.GetType() == domainEvent.GetType());

            if (existingEvent != null)
                _domainEvents.Remove(existingEvent);

            _domainEvents.Add(domainEvent);
        }
        public void ClearDomainEvents() => _domainEvents.Clear();

        protected BaseEntity()
        {

        }
    }
}
