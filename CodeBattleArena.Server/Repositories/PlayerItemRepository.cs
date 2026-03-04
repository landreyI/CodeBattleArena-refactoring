using CodeBattleArena.Server.Data;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Repositories.IRepositories;
using CodeBattleArena.Server.Specifications;
using CodeBattleArena.Server.Specifications.ItemSpec;
using Microsoft.EntityFrameworkCore;

namespace CodeBattleArena.Server.Repositories
{
    public class PlayerItemRepository : IPlayerItemRepository
    {
        private readonly AppDBContext _context;

        public PlayerItemRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<PlayerItem> GetPlayerItemAsync(ISpecification<PlayerItem> spec, CancellationToken cancellationToken)
        {
            var query = _context.PlayerItems.AsQueryable();
            query = SpecificationEvaluator.GetQuery(query, spec);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<List<PlayerItem>> GetListPlayerItemAsync(ISpecification<PlayerItem> spec, CancellationToken cancellationToken)
        {
            var query = _context.PlayerItems.AsQueryable();
            query = SpecificationEvaluator.GetQuery(query, spec);
            return await query.ToListAsync(cancellationToken);
        }
        public async Task AddPlayerItemAsync(PlayerItem playerItem, CancellationToken cancellationToken)
        {
            await _context.PlayerItems.AddAsync(playerItem, cancellationToken);
        }

        public async Task AddPlayersItemsAsync(List<PlayerItem> playersItems, CancellationToken cancellationToken)
        {
            await _context.PlayerItems.AddRangeAsync(playersItems, cancellationToken);
        }

        public async Task DeletePlayerItemAsync(int idItem, string idPlayer, CancellationToken cancellationToken)
        {
            var playerItem = await GetPlayerItemAsync(new PlayerItemSpec(idItem, idPlayer), cancellationToken);
            if (playerItem != null)
                _context.PlayerItems.Remove(playerItem);
        }

        public Task DeletePlayerItems(int idItem, CancellationToken cancellationToken)
        {
            var playerItems = _context.PlayerItems.Where(pi => pi.IdItem == idItem);
            if(playerItems != null)
                _context.PlayerItems.RemoveRange(playerItems);
            return Task.CompletedTask;
        }
    }
}
