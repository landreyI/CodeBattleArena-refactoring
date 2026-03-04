using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.Players;
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

            // сделать доменное событие
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

        public Result Update(string? name = null, string? description = null, Difficulty? difficulty = null, bool? isGeneratedAI = null)
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

            if (isGeneratedAI.HasValue)
                IsGeneratedAI = isGeneratedAI.Value;

            // сделать доменное событие
            return Result.Success();
        }

        public Result AddTestCase(string input, string expectedOutput, bool isHidden)
        {
            var testCaseResult = TestCase.Create(Id, input, expectedOutput, isHidden);

            if (testCaseResult.IsFailure)
            {
                return Result.Failure(testCaseResult.Error);
            }

            _testCases.Add(testCaseResult.Value!);
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
            return Result.Success();
        }

        public Result SyncTestCases(IEnumerable<(string Input, string ExpectedOutput)> newTests)
        {
            if (newTests == null || !newTests.Any())
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
                    // Обновляем существующий (EF Core сам проверит, изменились ли данные)
                    var resultUpdate = existingTest.Update(expectedOutput: output);
                    if (resultUpdate.IsFailure)
                        return Result.Failure(resultUpdate.Error);
                }
                else
                {
                    // Добавляем новый
                    var resultAdd = AddTestCase(input, output, isHidden: true);
                    if (resultAdd.IsFailure)
                        return Result.Failure(resultAdd.Error);
                }
            }

            return Result.Success();
        }

        public Result SyncTaskLanguages(IEnumerable<(Guid programmingLangId, string preparation, string verificationCode, bool isGeneratedAI)> taskLanguages)
        {
            if (taskLanguages == null || !taskLanguages.Any())
                return Result.Success();

            foreach (var (programmingLangId, preparation, verificationCode, isGeneratedAI) in taskLanguages)
            {
                var upsertResult = UpsertTaskLanguage(programmingLangId, preparation, verificationCode, isGeneratedAI);
                if (upsertResult.IsFailure)
                    return Result.Failure(upsertResult.Error);
            }

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
            return Result.Success();
        }
    }
}