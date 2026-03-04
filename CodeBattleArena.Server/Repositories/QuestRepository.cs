using CodeBattleArena.Server.Data;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Repositories.IRepositories;
using CodeBattleArena.Server.Specifications;
using Microsoft.EntityFrameworkCore;

namespace CodeBattleArena.Server.Repositories
{
    public class QuestRepository : IQuestRepository
    {
        private readonly AppDBContext _context;

        public QuestRepository(AppDBContext context)
        {
            _context = context;
        }
        
        public async Task<TaskPlay> GetTaskPlayAsync(ISpecification<TaskPlay> spec, CancellationToken cancellationToken)
        {
            var query = _context.TasksPlay.AsQueryable();
            query = SpecificationEvaluator.GetQuery(query, spec);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<List<TaskPlay>> GetListTaskPlayAsync
            (ISpecification<TaskPlay> spec, CancellationToken cancellationToken)
        {
            var query = _context.TasksPlay
                .AsQueryable();

            return await SpecificationEvaluator.GetQuery(query, spec)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<PlayerTaskPlay>> GetListPlayerTaskPlayAsync(ISpecification<PlayerTaskPlay> spec, CancellationToken cancellationToken)
        {
            var query = _context.PlayerTaskPlays
                .AsQueryable();

            return await SpecificationEvaluator.GetQuery(query, spec)
                .ToListAsync(cancellationToken);
        }
        public async Task<PlayerTaskPlay> GetPlayerTaskPlayAsync
            (ISpecification<PlayerTaskPlay> spec, CancellationToken cancellationToken)
        {
            var query = _context.PlayerTaskPlays.AsQueryable();
            query = SpecificationEvaluator.GetQuery(query, spec);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<List<Reward>> GetRewardsAsync(CancellationToken cancellationToken)
        {
            return await _context.Rewards
                .Include(r => r.Item)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<TaskPlayReward>> GetTaskPlayRewardsAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.TaskPlayRewards
                .Include(tpr => tpr.TaskPlay)
                .Include(tpr => tpr.Reward)
                .ThenInclude(r => r.Item)
                .Where(tpr => tpr.TaskPlayId == id)
                .ToListAsync(cancellationToken);
        }
        public async Task AddTaskPlayAsync(TaskPlay taskPlay, CancellationToken cancellationToken)
        {
            await _context.TasksPlay.AddAsync(taskPlay);
        }
        public async Task DeleteTaskPlayAsync(int id, CancellationToken cancellationToken)
        {
            var taskPlay = await _context.TasksPlay.FindAsync(id, cancellationToken);
            if (taskPlay != null) 
                _context.TasksPlay.Remove(taskPlay);
        }
        public Task UpdateTaskPlay(TaskPlay taskPlay)
        {
            _context.TasksPlay.Update(taskPlay);
            return Task.CompletedTask;
        }
        public async Task AddPlayerTaskPlayAsync(PlayerTaskPlay playerTaskPlay, CancellationToken cancellationToken)
        {
            await _context.PlayerTaskPlays.AddAsync(playerTaskPlay);
        }
        public async Task AddPlayerTaskPlaysAsync(List<PlayerTaskPlay> playerTaskPlays, CancellationToken cancellationToken)
        {
            await _context.PlayerTaskPlays.AddRangeAsync(playerTaskPlays);
        }
        public async Task DeletePlayerTaskPlayAsync(int id, CancellationToken cancellationToken)
        {
            var playerTaskPlay = await _context.PlayerTaskPlays.FindAsync(id, cancellationToken);
            if (playerTaskPlay != null)
                _context.PlayerTaskPlays.Remove(playerTaskPlay);
        }
        public Task UpdatePlayerTaskPlay(PlayerTaskPlay playerTaskPlay)
        {
            _context.PlayerTaskPlays.Update(playerTaskPlay);
            return Task.CompletedTask;
        }
        public Task UpdatePlayerTaskPlays(List<PlayerTaskPlay> playerTaskPlays)
        {
            _context.PlayerTaskPlays.UpdateRange(playerTaskPlays);
            return Task.CompletedTask;
        }
        public async Task AddRewardAsync(Reward reward, CancellationToken cancellationToken)
        {
            await _context.AddAsync(reward, cancellationToken);
        }
        public async Task DeleteRewardAsync(int id, CancellationToken cancellationToken)
        {
            var reward = await _context.Rewards.FindAsync(id, cancellationToken);
            if (reward != null)
                _context.Rewards.Remove(reward);
        }
        public Task DeleteRewards(List<TaskPlayReward> taskPlayRewards)
        {
            _context.TaskPlayRewards.RemoveRange(taskPlayRewards);
            return Task.CompletedTask;
        }
        public Task UpdateReward(Reward reward)
        {
            _context.Rewards.Update(reward);
            return Task.CompletedTask;
        }
    }
}
