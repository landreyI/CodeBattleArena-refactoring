namespace CodeBattleArena.Server.DTO.ModelsDTO
{
    public class RewardDto
    {
        public int? IdReward { get; set; }

        public int? ItemId { get; set; }
        public ItemDto? Item { get; set; }

        public int? Amount { get; set; } // Например, если награда — 100 очков или 5 предметов
        public string RewardType { get; set; } // "Item", "XP", "Currency", etc.
    }
}
