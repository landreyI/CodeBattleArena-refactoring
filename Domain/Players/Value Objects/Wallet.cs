
using CodeBattleArena.Domain.Common;

namespace CodeBattleArena.Domain.Players.Value_Objects
{
    public record Wallet
    {
        public int Coins { get; init; } = 0;

        public Wallet Add(int amount) => this with { Coins = Coins + amount };

        public Wallet Spend(int amount)
        {
            if (Coins < amount) throw new DomainException("Insufficient funds");
            return this with { Coins = Coins - amount };
        }
    }
}
