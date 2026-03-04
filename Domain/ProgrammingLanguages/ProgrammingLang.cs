using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.ProgrammingTasks;
using CodeBattleArena.Domain.Sessions;
using CodeBattleArena.Domain.TaskLanguages;

namespace CodeBattleArena.Domain.ProgrammingLanguages
{
    public class ProgrammingLang : BaseEntity<Guid>
    {
        public string Alias { get; private set; } // Например, "cpp", "csharp"
        public string Name { get; private set; } // Например, "C++", "C#"
        public string ExternalId { get; private set; }

        private readonly List<Session> _sessions = new();
        public virtual ICollection<Session> Sessions => _sessions.AsReadOnly();

        private readonly List<TaskLanguage> _taskLanguages = new();
        public virtual ICollection<TaskLanguage> TaskLanguages => _taskLanguages.AsReadOnly();

        private ProgrammingLang() { } // Для EF

        private ProgrammingLang(string alias, string name, string externalId)
        {
            Alias = alias;
            Name = name;
            ExternalId = externalId;
        }

        public static Result<ProgrammingLang> Create(string alias, string name, string externalId)
        {
            if (string.IsNullOrWhiteSpace(alias))
                return Result<ProgrammingLang>.Failure(new Error("lang.invalid_alias", "Alias cannot be empty (e.g., 'cpp', 'csharp')."));

            if (string.IsNullOrWhiteSpace(name))
                return Result<ProgrammingLang>.Failure(new Error("lang.invalid_name", "Language name cannot be empty."));

            if (string.IsNullOrWhiteSpace(externalId))
                return Result<ProgrammingLang>.Failure(new Error("lang.invalid_external_id", "External (Judge0) ID is required."));

            return Result<ProgrammingLang>.Success(new ProgrammingLang(alias, name, externalId));
        }

        public Result UpdateExternalId(string newId)
        {
            if (string.IsNullOrWhiteSpace(newId))
                return Result.Failure(new Error("lang.update_invalid_id", "New External ID cannot be empty."));

            ExternalId = newId;
            return Result.Success();
        }
    }
}