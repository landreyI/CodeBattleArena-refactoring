using CodeBattleArena.Server.Enums;
using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.Models
{
    public class TaskPlay
    {
        [Key]
        public int IdTask { get; set; }

        [StringLength(40)]
        public string Name { get; set; }
        public string Description { get; set; }

        [StringLength(40)]
        public TaskType Type { get; set; }
        public int? Experience { get; set; }
        public int? RewardCoin {  get; set; }
        public bool IsRepeatable { get; set; }
        public int? RepeatAfterDays { get; set; }

        public virtual ICollection<TaskPlayParam>? TaskPlayParams { get; set; }
        public virtual ICollection<TaskPlayReward>? TaskPlayRewards { get; set; }
        public virtual ICollection<PlayerTaskPlay>? PlayerTaskPlays { get; set; }
    }
}
