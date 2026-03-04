namespace CodeBattleArena.Server.Models
{
    public class TaskPlayReward
    {
        public int TaskPlayId { get; set; }
        public virtual TaskPlay? TaskPlay { get; set; }

        public int RewardId { get; set; }
        public virtual Reward? Reward { get; set; }
    }
}
