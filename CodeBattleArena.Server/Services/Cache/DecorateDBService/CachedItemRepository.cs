using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Repositories.IRepositories;

namespace CodeBattleArena.Server.Services.Cache.DecorateDBService
{
    public class CachedItemRepository : IItemRepository
    {
        private readonly IItemRepository _inner;
        private readonly ICacheService _cache;
        private const string CacheKeyPrefix = "item:";
        private const string CacheKeyList = "list";

        public CachedItemRepository(IItemRepository inner, ICacheService cache)
        {
            _inner = inner;
            _cache = cache;
        }
        public async Task RemoveCacheListAsync() => await _cache.RemoveAsync(CacheKeyPrefix + CacheKeyList);

        public async Task<Item> GetItemAsync(int id, CancellationToken ct)
        {
            var item = await _cache.GetAsync<Item>(CacheKeyPrefix + id);
            if (item != null)
                return item;

            item = await _inner.GetItemAsync(id, ct);
            if (item != null)
                await _cache.SetAsync(CacheKeyPrefix + item.IdItem, item);

            return item;
        }
        public async Task<List<Item>> GetItemsAsync(IFilter<Item>? filter, CancellationToken ct)
        {
            if (filter == null || filter.IsEmpty())
            {
                var items = await _cache.GetAsync<List<Item>>(CacheKeyPrefix + CacheKeyList);
                if (items != null)
                    return items;
            }

            var freshItems = await _inner.GetItemsAsync(filter, ct);

            if ((filter == null || filter.IsEmpty()) && freshItems != null)
                await _cache.SetAsync(CacheKeyPrefix + CacheKeyList, freshItems);

            return freshItems;
        }
        public async Task AddItemAsync(Item item, CancellationToken ct)
        {
            await _inner.AddItemAsync(item, ct);
            await RemoveCacheListAsync();
        }
        public async Task DeleteItemAsync(int id, CancellationToken ct)
        {
            await _inner.DeleteItemAsync(id, ct);
            await _cache.RemoveAsync(CacheKeyPrefix + id);
            await RemoveCacheListAsync();
        }
        public async Task UpdateItem(Item item)
        {
            await _inner.UpdateItem(item);
            await _cache.SetAsync(CacheKeyPrefix + item.IdItem, item);
            await RemoveCacheListAsync();
        }
    }
}
