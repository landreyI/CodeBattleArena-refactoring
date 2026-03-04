using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.ProgrammingTasks.Events.Integration
{
    public record ProgrammingTaskUpdatedIntegrationEvent(ProgrammingTask Task) : IIntegrationEvent;
}
