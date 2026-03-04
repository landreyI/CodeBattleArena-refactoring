
using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Domain.Common
{
    public abstract class BaseEntity<TId>
    {
        [Key]
        public TId Id { get; protected set; }


        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();

        protected BaseEntity()
        {

        }
    }
}
