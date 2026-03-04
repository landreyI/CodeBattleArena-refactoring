using CodeBattleArena.Server.Data;
using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CodeBattleArena.Server.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly AppDBContext _context;

        public ItemRepository(AppDBContext context)
        {
            _context = context;
        }
        public async Task<Item> GetItemAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Items.FirstOrDefaultAsync(i => i.IdItem == id, cancellationToken);
        }
        public async Task<List<Item>> GetItemsAsync(IFilter<Item>? filter, CancellationToken cancellationToken)
        {
            var query = _context.Items.AsQueryable();
            query = filter.ApplyTo(query);
            return await query.ToListAsync(cancellationToken);
        }
        public async Task AddItemAsync(Item item, CancellationToken cancellationToken)
        {
            await _context.Items.AddAsync(item);
        }
        public async Task DeleteItemAsync(int id, CancellationToken cancellationToken)
        {
            var item = await _context.Items.FindAsync(id, cancellationToken);
            if (item != null)
                _context.Items.Remove(item);
        }
        public Task UpdateItem(Item item)
        {
            _context.Items.Update(item);
            return Task.CompletedTask;
        }
    }
}
