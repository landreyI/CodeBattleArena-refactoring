namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public class PlayerItemDto
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public PlayerDto? Player { get; set; }
        public Guid PayerId { get; set; }
        public PlayerDto? Payer { get; set; }

        public Guid ItemId { get; set; }
        public ItemDto? Item { get; set; }

        public DateTime AcquiredAt { get; set; }
        public bool IsEquipped { get; set; }
    }
}
