using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.ProgrammingTasks.Events.Integration
{
    public record ProgrammingTaskDeletedIntegrationEvent(Guid Id) : IIntegrationEvent;
}
