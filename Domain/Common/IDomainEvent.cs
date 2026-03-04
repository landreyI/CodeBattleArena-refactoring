
namespace CodeBattleArena.Domain.Common
{
    // Базовый маркер для всех событий
    public interface IDomainEvent { }

    // Маркер для внутренних событий (в транзакции)
    public interface IInternalEvent : IDomainEvent { }

    // Маркер для внешних событий (SignalR/Integration)
    public interface IIntegrationEvent : IDomainEvent { }
}
