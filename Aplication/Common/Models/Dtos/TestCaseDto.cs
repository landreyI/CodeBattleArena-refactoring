namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public class TestCaseDto
    {
        public Guid Id { get; set; }
        public Guid ProgrammingTaskId { get; set; }
        public string Input { get; set; } = string.Empty;
        public string ExpectedOutput { get; set; } = string.Empty;
        public bool IsHidden { get; set; }
    }
}
