using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.Models
{
    public class Reward
    {
        [Key]
        public int IdReward { get; set; }

        public int? ItemId { get; set; }
        public virtual Item? Item { get; set; }

        public int? Amount { get; set; } // Например, если награда — 100 очков или 5 предметов

        [StringLength(50)]
        public string RewardType { get; set; } // "Item", "XP", "Currency", etc.
    }
}
