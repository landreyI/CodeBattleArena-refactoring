using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.PlayerItems;

namespace CodeBattleArena.Domain.Items
{
    public class Item : BaseEntity<Guid>
    {
        public string Name { get; private set; }
        public TypeItem Type { get; private set; }

        // Если null — предмет нельзя купить за монеты (например, награда за квест)
        public int? PriceCoin { get; private set; }

        // Метаданные для фронтенда
        public string? CssClass { get; private set; }
        public string? ImageUrl { get; private set; }
        public string? Description { get; private set; }

        private readonly List<PlayerItem> _playerItems = new();
        public virtual ICollection<PlayerItem> PlayerItems => _playerItems.AsReadOnly();

        private Item() { } // Для EF

        private Item(string name, TypeItem type, int? price)
        {
            Name = name;
            Type = type;
            PriceCoin = price;
        }

        public static Result<Item> Create(string name, TypeItem type, int? price = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Item>.Failure(new Error("item.invalid_name", "Item name cannot be empty."));

            if (price.HasValue && price.Value < 0)
                return Result<Item>.Failure(new Error("item.invalid_price", "The price cannot be negative."));

            return Result<Item>.Success(new Item(name, type, price));
        }

        public Result UpdateDetails(string? description = default, string? imageUrl = default, string? cssClass = default)
        {
            Description = description;
            ImageUrl = imageUrl;
            CssClass = cssClass;

            return Result.Success();
        }

        public Result SetPrice(int? newPrice = default)
        {
            if (newPrice.HasValue && newPrice.Value < 0)
                return Result.Failure(new Error("item.invalid_price", "The price cannot be negative."));

            PriceCoin = newPrice;
            return Result.Success();
        }

        public bool IsPurchasable => PriceCoin.HasValue && PriceCoin > 0;
    }
}