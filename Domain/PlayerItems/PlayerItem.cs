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

        public DateTime AcquiredAt { get; private set; }
        public bool IsEquipped { get; private set; }

        private PlayerItem() { } // Для EF

        private PlayerItem(Guid playerId, Guid itemId)
        {
            PlayerId = playerId;
            ItemId = itemId;
            AcquiredAt = DateTime.UtcNow;
            IsEquipped = false;
        }

        public static Result<PlayerItem> Create(Guid playerId, Guid itemId)
        {
            if (playerId == Guid.Empty)
                return Result<PlayerItem>.Failure(new Error("player_item.no_player", "Player ID cannot be empty."));

            if (itemId == Guid.Empty)
                return Result<PlayerItem>.Failure(new Error("player_item.no_item", "Item ID cannot be empty."));

            return Result<PlayerItem>.Success(new PlayerItem(playerId, itemId));
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