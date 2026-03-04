using CodeBattleArena.Server.Enums;

namespace CodeBattleArena.Server.DTO.ModelsDTO
{
    public class ItemDto
    {
        public int? IdItem { get; set; }
        public string Name { get; set; }
        public TypeItem Type { get; set; }
        public int? PriceCoin { get; set; }
        public string? CssClass { get; set; } // стили на клиенте
        public string? ImageUrl { get; set; } // ссылка на картинку/превью

        public string? Description { get; set; }
    }
}
