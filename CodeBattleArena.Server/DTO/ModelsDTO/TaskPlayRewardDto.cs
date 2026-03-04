namespace CodeBattleArena.Server.DTO.ModelsDTO
{
    public class TaskPlayRewardDto
    {
        public int TaskPlayId { get; set; }
        public TaskPlayDto? TaskPlay { get; set; }

        public int RewardId { get; set; }
        public RewardDto? Reward { get; set; }
    }
}
