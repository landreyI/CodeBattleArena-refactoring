using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.Items.Events.Integration;
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

        private Item(
        string name,
        TypeItem type,
        int? price,
        string? imageUrl,
        string? cssClass,
        string? description)
        {
            Name = name;
            Type = type;
            PriceCoin = price;
            ImageUrl = imageUrl;
            CssClass = cssClass;
            Description = description;

            AddDomainEvent(new ItemCreatedIntegrationEvent(this));
        }

        public static Result<Item> Create(
            string name,
            TypeItem type,
            int? price = default,
            string? imageUrl = default,
            string? cssClass = default,
            string? description = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Item>.Failure(new Error("item.invalid_name", "Item name cannot be empty."));

            if (price.HasValue && price.Value < 0)
                return Result<Item>.Failure(new Error("item.invalid_price", "The price cannot be negative."));

            var finalImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();
            var finalCssClass = string.IsNullOrWhiteSpace(cssClass) ? null : cssClass.Trim();
            var finalDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

            return Result<Item>.Success(new Item(
                name,
                type,
                price,
                finalImageUrl,
                finalCssClass,
                finalDescription));
        }

        public Result Update(
            string? name,
            TypeItem? type,
            int? price = default,
            string? imageUrl = default,
            string? cssClass = default,
            string? description = default)
        {
            if (!string.IsNullOrWhiteSpace(name))
                Name = name;

            if(type.HasValue)
                Type = type.Value;

            if (price.HasValue && price.Value < 0)
                return Result<Item>.Failure(new Error("item.invalid_price", "The price cannot be negative."));
            if(price.HasValue)
                PriceCoin = price.Value;

            if (!string.IsNullOrWhiteSpace(imageUrl))
                ImageUrl = imageUrl;

            if (!string.IsNullOrWhiteSpace(cssClass))
                CssClass = cssClass;

            if (!string.IsNullOrWhiteSpace(description))
                Description = description;

            AddDomainEvent(new ItemUpdatedIntegrationEvent(this));

            return Result.Success();
        }

        public Result SetPrice(int? newPrice = default)
        {
            if (newPrice.HasValue && newPrice.Value < 0)
                return Result.Failure(new Error("item.invalid_price", "The price cannot be negative."));

            PriceCoin = newPrice;
            return Result.Success();
        }

        public void Delete()
        {
            // Мы передаем только Id, так как сущность скоро перестанет существовать
            AddDomainEvent(new ItemDeletedIntegrationEvent(this.Id));
        }

        public bool IsPurchasable => PriceCoin.HasValue && PriceCoin > 0;
    }
}