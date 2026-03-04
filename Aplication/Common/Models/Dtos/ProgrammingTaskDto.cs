namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public class ProgrammingTaskDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;

        public List<TaskLanguageDto>? TaskLanguages { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid AuthorId { get; set; }
        public PlayerDto? Author { get; set; }
        public bool IsGeneratedAI { get; set; }

        public List<TestCaseDto>? TestCases { get; set; }
    }
}
