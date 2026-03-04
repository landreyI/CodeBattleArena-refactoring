namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public class RewardDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public int Amount { get; set; }

        public Guid? ItemId { get; set; }
        public ItemDto? Item { get; set; }
    }
}
