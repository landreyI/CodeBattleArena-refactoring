using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Specifications;

namespace CodeBattleArena.Server.Repositories.IRepositories
{
    public interface IQuestRepository
    {
        Task<TaskPlay> GetTaskPlayAsync(ISpecification<TaskPlay> spec, CancellationToken cancellationToken);
        Task<List<TaskPlay>> GetListTaskPlayAsync(ISpecification<TaskPlay> spec, CancellationToken cancellationToken);
        Task<List<PlayerTaskPlay>> GetListPlayerTaskPlayAsync(ISpecification<PlayerTaskPlay> spec, CancellationToken cancellationToken);
        Task<PlayerTaskPlay> GetPlayerTaskPlayAsync(ISpecification<PlayerTaskPlay> spec, CancellationToken cancellationToken);
        Task<List<Reward>> GetRewardsAsync(CancellationToken cancellationToken);
        Task<List<TaskPlayReward>> GetTaskPlayRewardsAsync(int id, CancellationToken cancellationToken);

        Task AddTaskPlayAsync(TaskPlay taskPlay, CancellationToken cancellationToken);
        Task DeleteTaskPlayAsync(int id, CancellationToken cancellationToken);
        Task UpdateTaskPlay(TaskPlay taskPlay);

        Task AddPlayerTaskPlayAsync(PlayerTaskPlay playerTaskPlay, CancellationToken cancellationToken);
        Task AddPlayerTaskPlaysAsync(List<PlayerTaskPlay> playerTaskPlays, CancellationToken cancellationToken);
        Task DeletePlayerTaskPlayAsync(int id, CancellationToken cancellationToken);
        Task UpdatePlayerTaskPlay(PlayerTaskPlay playerTaskPlay);
        Task UpdatePlayerTaskPlays(List<PlayerTaskPlay> playerTaskPlays);

        Task AddRewardAsync(Reward reward, CancellationToken cancellationToken);
        Task DeleteRewardAsync(int id, CancellationToken cancellationToken);
        Task DeleteRewards(List<TaskPlayReward> taskPlayRewards);
        Task UpdateReward(Reward reward);
    }
}
