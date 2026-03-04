using CodeBattleArena.Server.Data;
using CodeBattleArena.Server.DTO;
using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CodeBattleArena.Server.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly AppDBContext _context;
        private readonly UserManager<Player> _userManager;
        public PlayerRepository(AppDBContext context, UserManager<Player> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Player> GetPlayerAsync(string id, CancellationToken cancellationToken)
        {
            var player = await _context.Users
                .Include(u => u.ActiveBackground)
                .Include(u => u.ActiveBorder)
                .Include(u => u.ActiveAvatar)
                .Include(u => u.ActiveBadge)
                .Include(u => u.ActiveTitle)
                .Include(u => u.PlayerSessions)
                    .ThenInclude(ps => ps.Session)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            return player;
        }

        public async Task<List<Player>> GetPlayersAsync(IFilter<Player>? filter, CancellationToken cancellationToken)
        {
            var query = _context.Users
                .Include(u => u.ActiveBackground)
                .Include(u => u.ActiveBorder)
                .Include(u => u.ActiveAvatar)
                .Include(u => u.ActiveBadge)
                .Include(u => u.ActiveTitle)
                .AsQueryable();

            if (filter != null)
                query = filter.ApplyTo(query);

            var users = await query.ToListAsync(cancellationToken);

            if (filter is PlayerFilter playerFilter && !string.IsNullOrEmpty(playerFilter.Role))
            {
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    user.Roles = roles.ToList();
                }

                users = users.Where(u => u.Roles.Contains(playerFilter.Role)).ToList();
            }

            return users;
        }

        public async Task AddVictoryPlayerAsync(string id, CancellationToken cancellationToken)
        {
            var player = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (player != null) player.Victories++;
        }

        public async Task AddCountGamePlayerAsync(string id, CancellationToken cancellationToken)
        {
            var player = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            player.CountGames = (player.CountGames ?? 0) + 1;
        }

        public async Task<List<Session>> MyGamesListAsync(string id, CancellationToken cancellationToken)
        {
            return await _context.PlayersSession
                    .Where(u => u.IdPlayer == id)
                    .Select(ps => ps.Session)
                    .ToListAsync(cancellationToken);
        }
    }
}
