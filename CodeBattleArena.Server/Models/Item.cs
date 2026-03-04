using CodeBattleArena.Server.Enums;
using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Server.Models
{
    public class Item
    {
        [Key]
        public int IdItem { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(20)]
        public TypeItem Type { get; set; }
        public int? PriceCoin { get; set; }
        public string? CssClass { get; set; } // стили на клиенте
        public string? ImageUrl { get; set; } // ссылка на картинку/превью

        public string? Description { get; set; }

        public virtual ICollection<PlayerItem>? PlayerItems { get; set; }
    }
}
