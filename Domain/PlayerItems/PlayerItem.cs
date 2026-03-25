using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Items;
using CodeBattleArena.Domain.Players;

namespace CodeBattleArena.Domain.PlayerItems
{
    public class PlayerItem : BaseEntity<Guid>
    {
        public Guid PlayerId { get; private set; }
        public virtual Player? Player { get; private set; }

        public Guid ItemId { get; private set; }
        public virtual Item? Item { get; private set; }

        public Guid PayerId { get; private set; }
        public virtual Player? Payer { get; private set; }

        public DateTime AcquiredAt { get; private set; }
        public bool IsEquipped { get; private set; }

        public bool IsGift => PlayerId != PayerId;

        private PlayerItem() { } // Для EF

        private PlayerItem(Guid playerId, Guid itemId, Guid payerId)
        {
            PlayerId = playerId;
            ItemId = itemId;
            PayerId = payerId;
            AcquiredAt = DateTime.UtcNow;
            IsEquipped = false;

            // Здесь можно добавить доменное событие - оповещение новом предмете
        }

        public static Result<PlayerItem> Create(Guid playerId, Guid itemId, Guid payerId)
        {
            if (playerId == Guid.Empty)
                return Result<PlayerItem>.Failure(new Error("player_item.no_player", "Player ID cannot be empty."));

            if (itemId == Guid.Empty)
                return Result<PlayerItem>.Failure(new Error("player_item.no_item", "Item ID cannot be empty."));

            if (payerId == Guid.Empty)
                return Result<PlayerItem>.Failure(new Error("player_item.no_payer", "Payer ID cannot be empty."));

            return Result<PlayerItem>.Success(new PlayerItem(playerId, itemId, payerId));
        }

        public Result Equip()
        {
            if (IsEquipped)
                return Result.Failure(new Error("player_item.already_equipped", "This item is already equipped."));

            IsEquipped = true;
            return Result.Success();
        }

        public Result Unequip()
        {
            if (!IsEquipped)
                return Result.Failure(new Error("player_item.already_unequipped", "This item is not equipped."));

            IsEquipped = false;
            return Result.Success();
        }
    }
}