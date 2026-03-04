using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.Players;
using CodeBattleArena.Domain.ProgrammingTasks.Events.Integration;
using CodeBattleArena.Domain.ProgrammingTasks.Value_Objects;
using CodeBattleArena.Domain.Sessions;
using CodeBattleArena.Domain.TaskLanguages;
using CodeBattleArena.Domain.TestCases;

namespace CodeBattleArena.Domain.ProgrammingTasks
{
    public class ProgrammingTask : BaseEntity<Guid>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Difficulty Difficulty { get; private set; }

        private readonly List<TaskLanguage> _taskLanguages = new();
        public virtual ICollection<TaskLanguage> TaskLanguages => _taskLanguages.AsReadOnly();

        public DateTime CreatedAt { get; private set; }

        public Guid AuthorId { get; private set; }

        public bool IsGeneratedAI { get; private set; }

        public virtual Player? Author { get; private set; }

        private readonly List<TestCase> _testCases = new();
        public virtual ICollection<TestCase> TestCases => _testCases.AsReadOnly();

        private readonly List<Session> _sessions = new();
        public virtual ICollection<Session> Sessions => _sessions.AsReadOnly();

        private ProgrammingTask() { } // Для EF

        // 1. Приватный конструктор
        private ProgrammingTask(
            string name,
            string description,
            Difficulty difficulty,
            Guid authorId,
            bool isGeneratedAI)
        {
            Name = name;
            Description = description;
            Difficulty = difficulty;
            AuthorId = authorId;
            CreatedAt = DateTime.UtcNow;
            IsGeneratedAI = isGeneratedAI;

            AddDomainEvent(new ProgrammingTaskCreatedIntegrationEvent(this));
        }

        public static Result<ProgrammingTask> Create(
            string name,
            string description,
            Difficulty difficulty,
            Guid authorId,
            bool isGeneratedAI = false)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<ProgrammingTask>.Failure(new Error("task.invalid_name", "Task name cannot be empty."));

            if (string.IsNullOrWhiteSpace(description))
                return Result<ProgrammingTask>.Failure(new Error("task.invalid_description", "Task description cannot be empty."));

            if (authorId == Guid.Empty)
                return Result<ProgrammingTask>.Failure(new Error("task.invalid_author_id", "Author ID is required."));

            return Result<ProgrammingTask>.Success(new ProgrammingTask(
                name, description, difficulty, authorId, isGeneratedAI));
        }

        public Result Update(string? name = null, string? description = null, Difficulty? difficulty = null, bool isGeneratedAI = false)
        {
            if(name != null)
            {
                if (string.IsNullOrWhiteSpace(name))
                    return Result.Failure(new Error("task.invalid_name", "Task name cannot be empty."));
                Name = name;
            }

            if(description != null)
            {
                if (string.IsNullOrWhiteSpace(description))
                    return Result.Failure(new Error("task.invalid_description", "Task description cannot be empty."));
                Description = description;
            }

            if (difficulty.HasValue)
                Difficulty = difficulty.Value;

            AddDomainEvent(new ProgrammingTaskUpdatedIntegrationEvent(this));
            return Result.Success();
        }

        public void Delete()
        {
            // Мы передаем только Id, так как сущность скоро перестанет существовать
            AddDomainEvent(new ProgrammingTaskDeletedIntegrationEvent(this.Id));
        }
        public Result AddTestCase(string input, string expectedOutput, bool isHidden)
        {
            var testCaseResult = TestCase.Create(Id, input, expectedOutput, isHidden);

            if (testCaseResult.IsFailure)
            {
                return Result.Failure(testCaseResult.Error);
            }

            _testCases.Add(testCaseResult.Value!);

            AddDomainEvent(new ProgrammingTaskUpdatedIntegrationEvent(this));
            return Result.Success();
        }

        public Result AddLanguage(Guid langId, string preparation, string verificationCode, bool isGeneratedAI)
        {
            if (_taskLanguages.Any(tl => tl.ProgrammingLangId == langId))
                return Result.Failure(new Error("task.duplicate_language", "This language is already added to the task."));

            var taskLangResult = TaskLanguage.Create(Id, langId, preparation, verificationCode, isGeneratedAI);

            if (taskLangResult.IsFailure)
                return Result.Failure(taskLangResult.Error);

            _taskLanguages.Add(taskLangResult.Value!);

            AddDomainEvent(new ProgrammingTaskUpdatedIntegrationEvent(this));
            return Result.Success();
        }

        public Result SyncTestCases(IEnumerable<TestCaseInfo>? newTests)
        {
            if (newTests == null)
                return Result.Success();
            var newTestsList = newTests.ToList();
            var inputsFromAi = newTestsList.Select(x => x.Input).ToHashSet();

            // Удаляем то, чего больше нет
            _testCases.RemoveAll(existing => !inputsFromAi.Contains(existing.Input));

            foreach (var (input, output) in newTestsList)
            {
                var existingTest = _testCases.FirstOrDefault(x => x.Input == input);

                if (existingTest != null)
                {
                    var resultUpdate = existingTest.Update(expectedOutput: output);
                    if (resultUpdate.IsFailure)
                        return Result.Failure(resultUpdate.Error);
                }
                else
                {
                    var resultAdd = AddTestCase(input, output, isHidden: true);
                    if (resultAdd.IsFailure)
                        return Result.Failure(resultAdd.Error);
                }
            }

            AddDomainEvent(new ProgrammingTaskUpdatedIntegrationEvent(this));
            return Result.Success();
        }

        public Result SyncTaskLanguages(IEnumerable<TaskLanguageInfo>? taskLanguages)
        {
            if (taskLanguages == null)
                return Result.Success();
            var newLeanguagesList = taskLanguages.ToList();
            var lengIdFromAi = newLeanguagesList.Select(x => x.ProgrammingLangId).ToHashSet();

            // Удаляем то, чего больше нет
            _taskLanguages.RemoveAll(existing => !lengIdFromAi.Contains(existing.ProgrammingLangId));

            foreach (var (programmingLangId, preparation, verificationCode, isGeneratedAI) in taskLanguages)
            {
                var upsertResult = UpsertTaskLanguage(programmingLangId, preparation, verificationCode, isGeneratedAI);
                if (upsertResult.IsFailure)
                    return Result.Failure(upsertResult.Error);
            }

            AddDomainEvent(new ProgrammingTaskUpdatedIntegrationEvent(this));
            return Result.Success();
        }

        public Result UpsertTaskLanguage(Guid programmingLangId, string preparation, string verificationCode, bool isGeneratedAI)
        {
            var existingLang = _taskLanguages.FirstOrDefault(tl => tl.ProgrammingLangId == programmingLangId);
            if (existingLang != null)
            {
                var updateResult = existingLang.Update(preparation, verificationCode, isGeneratedAI);
                if (updateResult.IsFailure)
                    return Result.Failure(updateResult.Error);
            }
            else
            {
                var addResult = AddLanguage(programmingLangId, preparation, verificationCode, isGeneratedAI);
                if (addResult.IsFailure)
                    return Result.Failure(addResult.Error);
            }

            AddDomainEvent(new ProgrammingTaskUpdatedIntegrationEvent(this));
            return Result.Success();
        }
    }
}