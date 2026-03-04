using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Repositories.IRepositories;
using CodeBattleArena.Server.Specifications;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CodeBattleArena.Server.Services.Cache.DecorateDBService
{
    public class CachedQuestRepository : IQuestRepository
    {
        private readonly IQuestRepository _inner;
        private readonly ICacheService _cache;
        private const string CacheKeyPrefix = "quest:";
        private const string CacheKeyList = "list";

        public CachedQuestRepository(IQuestRepository inner, ICacheService cache)
        {
            _inner = inner;
            _cache = cache;
        }
        public async Task RemoveCacheListAsync() => await _cache.RemoveAsync(CacheKeyPrefix + CacheKeyList);


        public Task<List<PlayerTaskPlay>> GetListPlayerTaskPlayAsync(ISpecification<PlayerTaskPlay> spec, CancellationToken ct)
            => _inner.GetListPlayerTaskPlayAsync(spec, ct);
        public Task<PlayerTaskPlay> GetPlayerTaskPlayAsync(ISpecification<PlayerTaskPlay> spec, CancellationToken ct)
            => _inner.GetPlayerTaskPlayAsync(spec, ct);
        public Task<List<Reward>> GetRewardsAsync(CancellationToken ct)
            => _inner.GetRewardsAsync(ct);
        public Task<List<TaskPlayReward>> GetTaskPlayRewardsAsync(int id, CancellationToken ct)
            => _inner.GetTaskPlayRewardsAsync(id, ct);
        public Task AddPlayerTaskPlayAsync(PlayerTaskPlay playerTaskPlay, CancellationToken ct)
            => _inner.AddPlayerTaskPlayAsync(playerTaskPlay, ct);
        public Task AddPlayerTaskPlaysAsync(List<PlayerTaskPlay> playerTaskPlays, CancellationToken ct)
            => _inner.AddPlayerTaskPlaysAsync(playerTaskPlays, ct);
        public Task DeletePlayerTaskPlayAsync(int id, CancellationToken ct)
            => _inner.DeletePlayerTaskPlayAsync(id, ct);
        public Task UpdatePlayerTaskPlay(PlayerTaskPlay playerTaskPlay)
            => _inner.UpdatePlayerTaskPlay(playerTaskPlay);
        public Task UpdatePlayerTaskPlays(List<PlayerTaskPlay> playerTaskPlays)
            => _inner.UpdatePlayerTaskPlays(playerTaskPlays);
        public Task AddRewardAsync(Reward reward, CancellationToken ct)
            => _inner.AddRewardAsync(reward, ct);
        public Task DeleteRewardAsync(int id, CancellationToken ct)
            => _inner.DeleteRewardAsync(id, ct);
        public Task DeleteRewards(List<TaskPlayReward> taskPlayRewards)
            => _inner.DeleteRewards(taskPlayRewards);
        public Task UpdateReward(Reward reward)
            => _inner.UpdateReward(reward);


        public async Task<TaskPlay> GetTaskPlayAsync(ISpecification<TaskPlay> spec, CancellationToken ct)
        {
            var cacheKey = spec.GetCacheKey();
            if (!string.IsNullOrEmpty(cacheKey))
            {
                var cached = await _cache.GetAsync<TaskPlay>(CacheKeyPrefix + cacheKey);
                if (cached != null)
                    return cached;

                var taskPlay = await _inner.GetTaskPlayAsync(spec, ct);
                if (taskPlay != null)
                    await _cache.SetAsync(CacheKeyPrefix + cacheKey, taskPlay);

                return taskPlay;
            }

            // Спецификация не поддерживает кэш
            return await _inner.GetTaskPlayAsync(spec, ct);
        }
        public async Task<List<TaskPlay>> GetListTaskPlayAsync(ISpecification<TaskPlay> spec, CancellationToken ct)
        {
            var cacheKey = spec.GetCacheKey();
            var cached = await _cache.GetAsync<List<TaskPlay>>(CacheKeyPrefix + CacheKeyList + cacheKey);
            if (cached != null)
                return cached;

            var taskPlayList = await _inner.GetListTaskPlayAsync(spec, ct);
            if (taskPlayList != null)
                await _cache.SetAsync(CacheKeyPrefix + CacheKeyList + cacheKey, taskPlayList);

            return taskPlayList;
        }
        public async Task AddTaskPlayAsync(TaskPlay taskPlay, CancellationToken ct)
        {
            await _inner.AddTaskPlayAsync(taskPlay, ct);
            await RemoveCacheListAsync();
        }
        public async Task DeleteTaskPlayAsync(int id, CancellationToken ct)
        {
            await _inner.DeleteTaskPlayAsync(id, ct);
            await _cache.RemoveAsync(CacheKeyPrefix + id);
            await RemoveCacheListAsync();
        }
        public async Task UpdateTaskPlay(TaskPlay taskPlay)
        {
            await _inner.UpdateTaskPlay(taskPlay);
            await _cache.SetAsync(CacheKeyPrefix + taskPlay.IdTask, taskPlay);
            await RemoveCacheListAsync();
        }
    }
}
