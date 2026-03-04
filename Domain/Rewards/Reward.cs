
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.Items;

namespace CodeBattleArena.Domain.Rewards
{
    public class Reward : BaseEntity<Guid>
    {
        public RewardType Type { get; private set; }
        public int Amount { get; private set; }

        public Guid? ItemId { get; private set; }
        public virtual Item? Item { get; private set; }

        private Reward() { } // Для EF

        private Reward(RewardType type, int amount, Guid? itemId = default)
        {
            
            Type = type;
            Amount = amount;
            ItemId = itemId;
        }

        public static Result<Reward> CreateXp(int amount)
        {
            if (amount <= 0)
                return Result<Reward>.Failure(new Error("reward.invalid_xp", "The amount of experience must be greater than zero."));

            return Result<Reward>.Success(new Reward(RewardType.XP, amount));
        }

        public static Result<Reward> CreateCoin(int amount)
        {
            if (amount <= 0)
                return Result<Reward>.Failure(new Error("reward.invalid_coins", "The number of coins must be greater than zero."));

            return Result<Reward>.Success(new Reward(RewardType.Coin, amount));
        }

        public static Result<Reward> CreateItem(Guid itemId, int amount = 1)
        {
            if (itemId == Guid.Empty)
                return Result<Reward>.Failure(new Error("reward.invalid_item", "Item ID cannot be empty"));

            if (amount <= 0)
                return Result<Reward>.Failure(new Error("reward.invalid_amount", "The number of items must be greater than zero."));

            return Result<Reward>.Success(new Reward(RewardType.Item, amount, itemId));
        }
    }
}
