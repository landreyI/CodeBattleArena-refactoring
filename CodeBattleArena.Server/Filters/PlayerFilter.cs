using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Filters
{
    public class PlayerFilter : IFilter<Player>
    {
        public string? Role { get; set; }
        public string? UserName { get; set; }

        public IQueryable<Player> ApplyTo(IQueryable<Player> query)
        {
            if (!string.IsNullOrEmpty(UserName))
                query = query.Where(p => p.UserName.ToLower().Contains(UserName.ToLower()));

            return query;
        }
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(UserName) && string.IsNullOrEmpty(Role);
        }
    }
}
