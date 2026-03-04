using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Specifications;
using CodeBattleArena.Server.Untils;

namespace CodeBattleArena.Server.Services.DBServices.IDBServices
{
    public interface IQuestService
    {
        Task<TaskPlay> GetTaskPlayAsync(ISpecification<TaskPlay> spec, CancellationToken cancellationToken);
        Task<List<TaskPlay>> GetListTaskPlayAsync(ISpecification<TaskPlay> spec, CancellationToken cancellationToken);
        Task<List<PlayerTaskPlay>> GetListPlayerTaskPlayAsync(ISpecification<PlayerTaskPlay> spec, CancellationToken cancellationToken);
        Task<PlayerTaskPlay> GetPlayerTaskPlayAsync(ISpecification<PlayerTaskPlay> spec, CancellationToken cancellationToken);
        Task<List<Reward>> GetRewardsAsync(CancellationToken cancellationToken);
        Task<List<TaskPlayReward>> GetTaskPlayRewardsAsync(int id, CancellationToken cancellationToken);
        Task<Result<Unit, ErrorResponse>> ClaimRewardAsync(string idPlayer, int idTaskPlay, CancellationToken cancellationToken);
        Task<Result<Unit, ErrorResponse>> UpdateOrResetTaskProgress(CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> AddTaskPlayAsync(TaskPlay taskPlay, List<int> idRewards, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> UpdateTaskPlayAsync(TaskPlayDto dto, List<int> idRewards, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> EnsurePlayerTaskPlayExistsForType(string idPlayer, TaskType taskType, CancellationToken cancellationToken);
        Task<Result<Unit, ErrorResponse>> AddTaskPlayInDBAsync(TaskPlay taskPlay, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> DeleteTaskPlayAsync(int id, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> UpdateTaskPlayInDbAsync(TaskPlay taskPlay, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> AddPlayerTaskPlayInDbAsync(PlayerTaskPlay playerTaskPlay, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> AddPlayerTaskPlaysInDbAsync(List<PlayerTaskPlay> playerTaskPlays, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> DeletePlayerTaskPlayInDbAsync(int id, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> UpdatePlayerTaskPlayInDbAsync(PlayerTaskPlay playerTaskPlay, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> UpdatePlayerTaskPlaysInBdAsync(List<PlayerTaskPlay> playerTaskPlays, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> AddRewardAsync(Reward reward, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> DeleteRewardAsync(int id, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> UpdateRewardAsync(Reward reward, CancellationToken cancellationToken, bool commit = true);
    }
}
