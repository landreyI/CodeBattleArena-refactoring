using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Models;
using System.Runtime.CompilerServices;

namespace CodeBattleArena.Server.Filters
{
    public class ItemFilter : IFilter<Item>
    {
        public string? Name { get; set; }
        public TypeItem? Type { get; set; }
        public int? Coin { get; set; }
        public bool IsCoinDescending { get; set; }
        public IQueryable<Item> ApplyTo(IQueryable<Item> query)
        {
            if(Type.HasValue)
                query = query.Where(x => x.Type == Type.Value);

            if (!string.IsNullOrEmpty(Name))
                query = query.Where(x => x.Name.ToLower().Contains(Name.ToLower()));

            if(Coin.HasValue)
                query = query.Where(x => x.PriceCoin <= Coin);

            if (!IsCoinDescending)
                query = query.OrderBy(x => x.PriceCoin);
            else
                query = query.OrderByDescending(x => x.PriceCoin);

            return query;
        }
        public bool IsEmpty()
        {
            return !Type.HasValue && string.IsNullOrEmpty(Name) && !Coin.HasValue && !IsCoinDescending;
        }
    }
}
