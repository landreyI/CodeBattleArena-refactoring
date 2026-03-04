namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public class QuestParamDto
    {
        public Guid Id { get; set; }
        public Guid QuestId { get; set; }
        public QuestDto? Quest { get; set; }

        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}
