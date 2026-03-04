namespace CodeBattleArena.Server.DTO.ModelsDTO
{
    public class PlayerItemDto
    {
        public string IdPlayer { get; set; }
        public PlayerDto? Player { get; set; }

        public int IdItem { get; set; }
        public ItemDto? Item { get; set; }
    }
}
