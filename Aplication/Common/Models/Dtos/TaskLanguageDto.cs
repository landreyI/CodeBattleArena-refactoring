namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public class TaskLanguageDto
    {
        public Guid Id { get; set; }
        public Guid ProgrammingTaskId { get; set; }
        public Guid ProgrammingLangId { get; set; }
        public string Preparation { get; set; } // Boilerplate код (например, "public class Solution { ... }")
        public string VerificationCode { get; set; } // Авторское решение

        public bool IsGeneratedAI { get; set; }
    }
}
