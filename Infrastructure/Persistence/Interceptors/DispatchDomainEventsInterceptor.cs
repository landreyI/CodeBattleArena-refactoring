using CodeBattleArena.Application.Common;
using CodeBattleArena.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CodeBattleArena.Infrastructure.Persistence.Interceptors
{
    public class DispatchDomainEventsInterceptor : SaveChangesInterceptor
    {
        private readonly IMediator _mediator;
        private readonly List<IIntegrationEvent> _integrationEvents = new();

        public DispatchDomainEventsInterceptor(IMediator mediator) => _mediator = mediator;

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
        {
            await DispatchEventsBeforeSave(eventData.Context, ct);
            return await base.SavingChangesAsync(eventData, result, ct);
        }

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData, int result, CancellationToken ct = default)
        {
            await DispatchEventsAfterSave(ct);
            return await base.SavedChangesAsync(eventData, result, ct);
        }

        private async Task DispatchEventsBeforeSave(DbContext? context, CancellationToken ct)
        {
            if (context == null) return;

            var entities = context.ChangeTracker.Entries<BaseEntity<Guid>>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity).ToList();

            var events = entities.SelectMany(e => e.DomainEvents).ToList();
            entities.ForEach(e => e.ClearDomainEvents());

            // 1. Внутренние события - публикуем ДО сохранения
            foreach (var @event in events.OfType<IInternalEvent>())
                await _mediator.Publish(CreateNotification(@event), ct);

            // 2. Внешние - откладываем
            _integrationEvents.AddRange(events.OfType<IIntegrationEvent>());
        }

        private async Task DispatchEventsAfterSave(CancellationToken ct)
        {
            // Публикуем ТОЛЬКО после успешного коммита в БД
            foreach (var @event in _integrationEvents)
                await _mediator.Publish(CreateNotification(@event), ct);

            _integrationEvents.Clear();
        }

        private static object CreateNotification(IDomainEvent @event)
        {
            var type = typeof(DomainEventNotification<>).MakeGenericType(@event.GetType());
            return Activator.CreateInstance(type, @event)!;
        }
    }
}
