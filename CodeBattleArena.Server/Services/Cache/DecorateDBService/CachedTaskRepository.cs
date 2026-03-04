using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Services.Cache.DecorateDBService
{
    public class CachedTaskRepository : ITaskRepository
    {
        private readonly ITaskRepository _inner;
        private readonly ICacheService _cache;
        private const string CacheKeyPrefix = "task:";
        private const string CacheKeyList = "list";

        public CachedTaskRepository(ITaskRepository inner, ICacheService cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public async Task RemoveCacheListAsync() => await _cache.RemoveAsync(CacheKeyPrefix + CacheKeyList);

        public Task AddInputDataAsync(string data, CancellationToken ct)
             => _inner.AddInputDataAsync(data, ct);
        public Task AddTaskInputDataAsync(TaskInputData taskInputData, CancellationToken ct)
            => _inner.AddTaskInputDataAsync(taskInputData, ct);
        public Task<List<InputData>> GetInputDataListAsync(CancellationToken ct)
            => _inner.GetInputDataListAsync(ct);
        public Task<List<TaskInputData>> GetTaskInputDataByIdTaskProgrammingAsync(int id, CancellationToken ct)
            => _inner.GetTaskInputDataByIdTaskProgrammingAsync(id, ct);
        public Task<InputData> GetInputDataById(int id)
            => _inner.GetInputDataById(id);
        public Task UpdateTaskInputDataAsync(TaskInputData taskInputData)
            => _inner.UpdateTaskInputDataAsync(taskInputData);
        public Task UpdateInputDataAsync(InputData inputData)
            => _inner.UpdateInputDataAsync(inputData);
        public Task DeleteTaskInputDataAsync(int idTaskProgramming, int idInputData, CancellationToken ct)
            => _inner.DeleteTaskInputDataAsync(idTaskProgramming, idInputData, ct);
        public Task DeleteListTaskInputDatas(List<TaskInputData> taskInputDatas)
            => _inner.DeleteListTaskInputDatas(taskInputDatas);

        public async Task<List<TaskProgramming>> GetTaskProgrammingListAsync(IFilter<TaskProgramming>? filter, CancellationToken ct)
        {
            if (filter == null || filter.IsEmpty())
            {
                var tasks = await _cache.GetAsync<List<TaskProgramming>>(CacheKeyPrefix + CacheKeyList);
                if (tasks != null)
                    return tasks;
            }

            var freshTasks = await _inner.GetTaskProgrammingListAsync(filter, ct);

            if ((filter == null || filter.IsEmpty()) && freshTasks != null)
                await _cache.SetAsync(CacheKeyPrefix + CacheKeyList, freshTasks);

            return freshTasks;
        }
        public async Task<TaskProgramming> GetTaskProgrammingAsync(int id, CancellationToken ct)
        {
            var task = await _cache.GetAsync<TaskProgramming>(CacheKeyPrefix + id);
            if (task != null)
                return task;

            task = await _inner.GetTaskProgrammingAsync(id, ct);
            if (task != null)
                await _cache.SetAsync(CacheKeyPrefix + task.IdTaskProgramming, task);

            return task;
        }
        public async Task UpdateTaskProgrammingAsync(TaskProgramming taskProgramming)
        {
            await _inner.UpdateTaskProgrammingAsync(taskProgramming);
            await _cache.SetAsync(CacheKeyPrefix + taskProgramming.IdTaskProgramming, taskProgramming);
            await RemoveCacheListAsync();
        }
        public async Task AddTaskProgrammingAsync(TaskProgramming taskProgramming, CancellationToken ct)
        {
            await _inner.AddTaskProgrammingAsync(taskProgramming, ct);
            await RemoveCacheListAsync();
        }
        public async Task DeleteTaskProgrammingAsync(int id, CancellationToken ct)
        {
            await _inner.DeleteTaskProgrammingAsync(id, ct);
            await _cache.RemoveAsync(CacheKeyPrefix + id);
            await RemoveCacheListAsync();
        }
    }
}
