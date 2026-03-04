using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.ProgrammingTasks.Events.Integration
{
    public record ProgrammingTaskCreatedIntegrationEvent(ProgrammingTask Task) : IIntegrationEvent;
}
