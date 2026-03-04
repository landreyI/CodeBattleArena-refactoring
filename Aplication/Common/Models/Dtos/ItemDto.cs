namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public class ItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        public int? PriceCoin { get; set; }

        public string? CssClass { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
    }
}
