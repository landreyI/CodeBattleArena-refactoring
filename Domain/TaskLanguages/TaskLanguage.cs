using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.ProgrammingLanguages;
using CodeBattleArena.Domain.ProgrammingTasks;

namespace CodeBattleArena.Domain.TaskLanguages
{
    public class TaskLanguage : BaseEntity<Guid>
    {
        public Guid ProgrammingTaskId { get; private set; }
        public virtual ProgrammingTask? ProgrammingTask { get; private set; }

        public Guid ProgrammingLangId { get; private set; }
        public virtual ProgrammingLang? ProgrammingLang { get; private set; }

        public string Preparation { get; private set; } // Boilerplate код (например, "public class Solution { ... }")
        public string VerificationCode { get; private set; } // Авторское решение

        public bool IsGeneratedAI { get; private set; }

        public TaskLanguage() { } // Для EF
        private TaskLanguage(
            Guid programmingTaskId, 
            Guid programmingLangId, 
            string preparation, 
            string verificationCode,
            bool isGeneratedAI)
        {
            ProgrammingTaskId = programmingTaskId;
            ProgrammingLangId = programmingLangId;
            Preparation = preparation;
            VerificationCode = verificationCode;
            IsGeneratedAI = isGeneratedAI;
        }

        public static Result<TaskLanguage> Create(Guid programmingTaskId, Guid programmingLangId, string preparation, string verificationCode, bool isGeneratedAI = false)
        {
            if (programmingTaskId == Guid.Empty)
                return Result<TaskLanguage>.Failure(new Error("task_language.invalid_programming_task", "Task ID cannot be empty."));

            if (programmingLangId == Guid.Empty)
                return Result<TaskLanguage>.Failure(new Error("task_language.invalid_programming_lang", "Language ID cannot be empty."));

            if (string.IsNullOrWhiteSpace(preparation))
                return Result<TaskLanguage>.Failure(new Error("task_language.invalid_preparation", "Preparation code cannot be empty."));

            if (string.IsNullOrWhiteSpace(verificationCode))
                return Result<TaskLanguage>.Failure(new Error("task_language.invalid_verification_code", "Verification code cannot be empty."));

            return Result<TaskLanguage>.Success(new TaskLanguage(programmingTaskId, programmingLangId, preparation, verificationCode, isGeneratedAI));
        }

        public Result Update(string? preparation = null, string? verificationCode = null, bool? isGeneratedAI = null)
        {
            if (preparation != null)
            {
                if (string.IsNullOrWhiteSpace(preparation))
                    return Result.Failure(new Error("task_language.invalid_preparation", "Preparation code cannot be empty."));
                Preparation = preparation;
            }

            if(verificationCode != null)
            {
                if (string.IsNullOrWhiteSpace(verificationCode))
                    return Result.Failure(new Error("task_language.invalid_verification_code", "Verification code cannot be empty."));
                VerificationCode = verificationCode;
            }

            if (isGeneratedAI.HasValue)
                IsGeneratedAI = isGeneratedAI.Value;

            return Result.Success();
        }
    }
}
