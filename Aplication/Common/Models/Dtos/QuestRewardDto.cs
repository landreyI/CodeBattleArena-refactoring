namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public class QuestRewardDto
    {
        public Guid Id { get; set; }
        public Guid QuestId { get; set; }
        public QuestDto? Quest { get; set; }

        public Guid RewardId { get; set; }
        public RewardDto? Reward { get; set; }
    }
}
