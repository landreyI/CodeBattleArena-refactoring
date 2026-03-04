
namespace CodeBattleArena.Application.Features.AI.Models
{
    public record AiGeneratedTaskDto
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public List<AiTaskLanguageDto> TaskLanguages { get; init; } = new();
        public List<AiTestCaseDto> TestCases { get; init; } = new();


    }

    public record AiTaskLanguageDto
    {
        public Guid ProgrammingLangId { get; init; }
        public string LanguageName { get; init; } = string.Empty;
        public string Preparation { get; init; } = string.Empty;
        public string VerificationCode { get; init; } = string.Empty;
        public string SolutionCode { get; init; } = string.Empty;
    }

    public record AiTestCaseDto
    {
        public string Input { get; init; } = string.Empty;
        public string ExpectedOutput { get; init; } = string.Empty;
    }
}
