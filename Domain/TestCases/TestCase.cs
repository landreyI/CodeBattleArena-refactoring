using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.ProgrammingTasks;

namespace CodeBattleArena.Domain.TestCases
{
    public class TestCase : BaseEntity<Guid>
    {
        public Guid ProgrammingTaskId { get; private set; }
        public ProgrammingTask? ProgrammingTask { get; set; }

        // Сами входные данные (то, что летит в STDIN)
        public string Input { get; private set; }

        // То, что мы ожидаем увидеть в STDOUT
        public string ExpectedOutput { get; private set; }

        // Если True — игрок не видит этот тест в интерфейсе
        public bool IsHidden { get; private set; }

        private TestCase() { } // Для EF

        private TestCase(Guid taskId, string input, string expectedOutput, bool isHidden = true)
        {
            ProgrammingTaskId = taskId;
            Input = input;
            ExpectedOutput = expectedOutput;
            IsHidden = isHidden;
        }

        public static Result<TestCase> Create(Guid taskId, string input, string expectedOutput, bool isHidden = true)
        {
            if (taskId == Guid.Empty)
                return Result<TestCase>.Failure(new Error("testcase.invalid_task_id", "Programming Task ID cannot be empty"));

            if (string.IsNullOrWhiteSpace(input))
                return Result<TestCase>.Failure(new Error("testcase.invalid_input", "Input cannot be empty or whitespace"));

            if (string.IsNullOrWhiteSpace(expectedOutput))
                return Result<TestCase>.Failure(new Error("testcase.invalid_expected_output", "Expected Output cannot be empty or whitespace"));

            return Result<TestCase>.Success(new TestCase(taskId, input, expectedOutput, isHidden));
        }

        public Result Update(string? input = null, string? expectedOutput = null, bool? isHidden = null)
        {
            if (input != null)
            {
                if (string.IsNullOrWhiteSpace(input))
                    return Result.Failure(new Error("testcase.invalid_input", "Input cannot be empty or whitespace"));
                Input = input;
            }

            if(expectedOutput != null)
            {
                if (string.IsNullOrWhiteSpace(expectedOutput))
                    return Result.Failure(new Error("testcase.invalid_expected_output", "Expected Output cannot be empty or whitespace"));
                ExpectedOutput = expectedOutput;
            }

            if (isHidden.HasValue)
                IsHidden = isHidden.Value;

            return Result.Success();
        }
    }
}
