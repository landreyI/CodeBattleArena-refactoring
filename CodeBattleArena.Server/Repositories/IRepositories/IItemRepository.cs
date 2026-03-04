using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Repositories.IRepositories
{
    public interface IItemRepository
    {
        Task<Item> GetItemAsync(int id, CancellationToken cancellationToken);
        Task<List<Item>> GetItemsAsync(IFilter<Item>? filter, CancellationToken cancellationToken);
        Task AddItemAsync(Item item, CancellationToken cancellationToken);
        Task DeleteItemAsync(int id, CancellationToken cancellationToken);
        Task UpdateItem(Item item);
    }
}
