using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.ProgrammingTasks;

namespace UnitTests.Common.DataBuilders
{
    public static class ProgrammingTaskBuilder
    {
        public static readonly Guid DefaultAuthorId = Guid.NewGuid();

        public static ProgrammingTask CreateDefault(string name = "Test Task")
        {
            var taskResult = ProgrammingTask.Create(
                name: name,
                description: "Default description",
                difficulty: Difficulty.Medium,
                authorId: DefaultAuthorId,
                isGeneratedAI: false
            );

            if (taskResult.IsFailure)
            {
                throw new InvalidOperationException(
                    $"[TestSetupError] Failed to create ProgrammingTask in Builder. " +
                    $"Error: {taskResult.Error}");
            }

            return taskResult.Value!;
        }

        public static List<ProgrammingTask> CreateList(int count)
        {
            return Enumerable.Range(1, count)
                .Select(i => CreateDefault($"Task {i}"))
                .ToList();
        }
    }
}
