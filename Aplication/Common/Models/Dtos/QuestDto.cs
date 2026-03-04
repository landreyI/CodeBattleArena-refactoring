namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public class QuestDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsRepeatable { get; set; }
        public int? RepeatAfterDays { get; set; } = default;
    }
}
