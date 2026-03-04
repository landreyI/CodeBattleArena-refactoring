namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public class PlayerQuestDto
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public PlayerDto? Player { get; set; }

        public Guid QuestId { get; set; }
        public QuestDto? Quest { get; set; }

        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsRewardClaimed { get; set; }
        public string ProgressValue { get; set; } = "0";
    }
}
