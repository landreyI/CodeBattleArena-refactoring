using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Helpers;
using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Specifications;
using CodeBattleArena.Server.Specifications.ItemSpec;
using CodeBattleArena.Server.Specifications.QuestSpec;
using CodeBattleArena.Server.Specifications.SessionSpec;
using CodeBattleArena.Server.Untils;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace CodeBattleArena.Server.Services.DBServices
{
    public class QuestService : IQuestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QuestService> _logger;
        private readonly IMapper _mapper;
        private readonly IPlayerService _playerService;
        private readonly IItemService _itemService;
        public QuestService(IUnitOfWork unitOfWork, ILogger<QuestService> logger, IMapper mapper,
            IPlayerService playerService, IItemService itemService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _playerService = playerService;
            _itemService = itemService;
        }

        public async Task<TaskPlay> GetTaskPlayAsync
            (ISpecification<TaskPlay> spec, CancellationToken cancellationToken)
        {
            return await _unitOfWork.QuestRepository.GetTaskPlayAsync(spec, cancellationToken);
        }
        public async Task<List<TaskPlay>> GetListTaskPlayAsync
            (ISpecification<TaskPlay> spec, CancellationToken cancellationToken)
        {
            return await _unitOfWork.QuestRepository.GetListTaskPlayAsync(spec, cancellationToken);
        }
        public async Task<List<PlayerTaskPlay>> GetListPlayerTaskPlayAsync
            (ISpecification<PlayerTaskPlay> spec, CancellationToken cancellationToken)
        {
            return await _unitOfWork.QuestRepository.GetListPlayerTaskPlayAsync(spec, cancellationToken);
        }
        public async Task<PlayerTaskPlay> GetPlayerTaskPlayAsync
            (ISpecification<PlayerTaskPlay> spec, CancellationToken cancellationToken)
        {
            var playerTaskPlay = await _unitOfWork.QuestRepository.GetPlayerTaskPlayAsync(spec, cancellationToken);
            if (playerTaskPlay != null && playerTaskPlay.IsCompleted)
            {
                if (QuestHelper.TryResetIfRepeatable(playerTaskPlay))
                {
                    QuestHelper.ResetPlayerTaskPlay(playerTaskPlay);
                    await UpdatePlayerTaskPlayInDbAsync(playerTaskPlay, cancellationToken);
                }
            }
            return playerTaskPlay;
        }
        public async Task<List<Reward>> GetRewardsAsync(CancellationToken cancellationToken)
        {
            return await _unitOfWork.QuestRepository.GetRewardsAsync(cancellationToken);
        }
        public async Task<List<TaskPlayReward>> GetTaskPlayRewardsAsync(int id, CancellationToken cancellationToken)
        {
            return await _unitOfWork.QuestRepository.GetTaskPlayRewardsAsync(id, cancellationToken);
        }

        public async Task<Result<Unit, ErrorResponse>> ClaimRewardAsync
            (string idPlayer, int idTaskPlay, CancellationToken cancellationToken)
        {
            var specPTP = Specification<PlayerTaskPlay>.Combine(
                new PlayerTaskPlayIncludesPlayer(),
                new PlayerTaskPlaySpec(idTaskPlay, idPlayer)
            );
            var playerTaskPlay = await GetPlayerTaskPlayAsync(specPTP, cancellationToken);
            var player = playerTaskPlay.Player;

            if (playerTaskPlay == null || player == null)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                { Error = "Database error when claim reward." });

            if (!playerTaskPlay.IsCompleted || playerTaskPlay.IsGet)
            {
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Task is not completed or reward already claimed."
                });
            }

            var playersItemsAdd = new List<PlayerItem>();
            var playersItemsCurrent = await _itemService.GetListPlayerItemByIdItemAsync
                (new PlayerItemSpec(idPlayer: player.Id), cancellationToken);

            var rewards = await _unitOfWork.QuestRepository.GetTaskPlayRewardsAsync(playerTaskPlay.TaskPlay.IdTask, cancellationToken);
            foreach (var taskPlayReward in rewards)
            {
                if (playersItemsCurrent.Any(pi => pi.IdItem == taskPlayReward.Reward.ItemId.Value))
                    continue;
                playersItemsAdd.Add(
                    new PlayerItem
                    {
                        IdItem = taskPlayReward.Reward.ItemId.Value,
                        IdPlayer = idPlayer,
                    }
                );
            }
            var resultAdding = await _itemService.AddPlayersItemsAsync(playersItemsAdd, cancellationToken, false);
            if (!resultAdding.IsSuccess)
                return resultAdding;

            playerTaskPlay.IsGet = true;

            var resultClaim = await UpdatePlayerTaskPlayInDbAsync(playerTaskPlay, cancellationToken);
            if (!resultClaim.IsSuccess)
                return resultClaim;

            player.Coin = (player.Coin ?? 0) + (playerTaskPlay.TaskPlay?.RewardCoin ?? 0);
            player.Experience = (player.Experience ?? 0) + (playerTaskPlay.TaskPlay?.Experience ?? 0);

            var resultUpdate = await _playerService.UpdatePlayerInDbAsync(player);
            if (!resultUpdate.IsSuccess)
                return resultUpdate;

            return Result.Success<Unit, ErrorResponse>(Unit.Value);
        }

        public async Task<Result<Unit, ErrorResponse>> UpdateOrResetTaskProgress
            (CancellationToken cancellationToken, bool commit = true)
        {
            var spec = Specification<PlayerTaskPlay>.Combine(
                new PlayerTaskPlaySpec(checkRepeatable: true)
            );
            var playersTaskPlays = await GetListPlayerTaskPlayAsync(spec, cancellationToken);
            foreach (var playerTaskPlay in playersTaskPlays)
            {
                if (QuestHelper.TryResetIfRepeatable(playerTaskPlay))
                {
                    QuestHelper.ResetPlayerTaskPlay(playerTaskPlay);
                }
            }
            var resultUpdate = await UpdatePlayerTaskPlaysInBdAsync(playersTaskPlays, cancellationToken, commit);
            if (!resultUpdate.IsSuccess) return resultUpdate;

            return Result.Success<Unit, ErrorResponse>(Unit.Value);
        }


        public async Task<Result<Unit, ErrorResponse>> AddTaskPlayAsync
            (TaskPlay taskPlay, List<int> idRewards, CancellationToken cancellationToken, bool commit = true)
        {
            List<TaskPlayReward> taskPlayRewards = idRewards.Select(id => new TaskPlayReward
            {
                RewardId = id,
            }).ToList();

            taskPlay.TaskPlayRewards = taskPlayRewards;
            var resultAdd = await AddTaskPlayInDBAsync(taskPlay, cancellationToken, commit);
            if(!resultAdd.IsSuccess)
                return Result.Failure<Unit, ErrorResponse>(resultAdd.Failure);

            return Result.Success<Unit, ErrorResponse>(Unit.Value);
        }
        public async Task<Result<Unit, ErrorResponse>> UpdateTaskPlayAsync
            (TaskPlayDto dto, List<int> idRewards, CancellationToken cancellationToken, bool commit = true)
        {
            var spec = Specification<TaskPlay>.Combine(
                new TaskPlayListsIncludesSpec(),
                new TaskPlaySpec(dto.IdTask.Value)
            );
            var taskPlay = await GetTaskPlayAsync(spec, cancellationToken);

            _mapper.Map(dto, taskPlay);

            var currentIds = taskPlay.TaskPlayRewards.Select(r => r.RewardId).ToHashSet();
            var newIds = idRewards.ToHashSet();

            var toAddIds = newIds.Except(currentIds).ToList();
            var toRemoveIds = currentIds.Except(newIds).ToList();

            // Удаляем из коллекции те, которые надо убрать
            foreach (var rewardId in toRemoveIds)
            {
                var itemToRemove = taskPlay.TaskPlayRewards.FirstOrDefault(r => r.RewardId == rewardId);
                if (itemToRemove != null)
                {
                    taskPlay.TaskPlayRewards.Remove(itemToRemove);
                }
            }

            // Добавляем новые
            foreach (var rewardId in toAddIds)
            {
                taskPlay.TaskPlayRewards.Add(new TaskPlayReward { RewardId = rewardId });
            }

            var resultUpdate = await UpdateTaskPlayInDbAsync(taskPlay, cancellationToken, commit);
            if (!resultUpdate.IsSuccess)
                return Result.Failure<Unit, ErrorResponse>(resultUpdate.Failure);

            return Result.Success<Unit, ErrorResponse>(Unit.Value);
        }
       
        public async Task<Result<Unit, ErrorResponse>> EnsurePlayerTaskPlayExistsForType(
            string idPlayer, TaskType taskType, CancellationToken cancellationToken)
        {
            var tasksPlays = await GetListTaskPlayAsync(new TaskPlayByParamKeySpec(taskType: taskType), cancellationToken);

            if (!tasksPlays.Any())
            {
                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }

            var spec = Specification<PlayerTaskPlay>.Combine(
                new PlayerTaskPlayByTypeSpec(taskType),
                new PlayerTaskPlaySpec(idPlayer: idPlayer)
            );
            var playersTasks = await GetListPlayerTaskPlayAsync(spec, cancellationToken);

            var addPlayerTaskPlay = tasksPlays
                .Where(tp => !playersTasks.Any(pt => pt.TaskPlayId == tp.IdTask))
                .Select(tp => new PlayerTaskPlay
                {
                    TaskPlayId = tp.IdTask,
                    PlayerId = idPlayer
                })
                .ToList();

            if (addPlayerTaskPlay.Any())
            {
                try
                {
                    await AddPlayerTaskPlaysInDbAsync(addPlayerTaskPlay, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error when adding PlayerTaskPlay");
                    return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                    {
                        Error = "Database error when adding PlayerTaskPlay."
                    });
                }
            }

            return Result.Success<Unit, ErrorResponse>(Unit.Value);
        }

        public async Task<Result<Unit, ErrorResponse>> AddTaskPlayInDBAsync
            (TaskPlay taskPlay, CancellationToken cancellationToken, bool commit = true)
        {
            try
            {
                await _unitOfWork.QuestRepository.AddTaskPlayAsync(taskPlay, cancellationToken);
                if (commit)
                    await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when adding TaskPlay");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when adding TaskPlay."
                });
            }
        }
        public async Task<Result<Unit, ErrorResponse>> DeleteTaskPlayAsync
            (int id, CancellationToken cancellationToken, bool commit = true)
        {
            try
            {
                await _unitOfWork.QuestRepository.DeleteTaskPlayAsync(id, cancellationToken);
                if (commit)
                    await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting TaskPlay");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when deleting TaskPlay."
                });
            }
        }
        public async Task<Result<Unit, ErrorResponse>> UpdateTaskPlayInDbAsync
            (TaskPlay taskPlay, CancellationToken cancellationToken, bool commit = true)
        {
            try
            {
                await _unitOfWork.QuestRepository.UpdateTaskPlay(taskPlay);
                if (commit)
                    await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating TaskPlay");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when updating TaskPlay."
                });
            }
        }

        public async Task<Result<Unit, ErrorResponse>> AddPlayerTaskPlayInDbAsync
            (PlayerTaskPlay playerTaskPlay, CancellationToken cancellationToken, bool commit = true)
        {
            try
            {
                await _unitOfWork.QuestRepository.AddPlayerTaskPlayAsync(playerTaskPlay, cancellationToken);
                if (commit)
                    await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when adding PlayerTaskPlay");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when adding PlayerTaskPlay."
                });
            }
        }
        public async Task<Result<Unit, ErrorResponse>> AddPlayerTaskPlaysInDbAsync
            (List<PlayerTaskPlay> playerTaskPlays, CancellationToken cancellationToken, bool commit = true)
        {
            try
            {
                await _unitOfWork.QuestRepository.AddPlayerTaskPlaysAsync(playerTaskPlays, cancellationToken);
                if (commit)
                    await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when adding PlayerTaskPlay");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when adding PlayerTaskPlay."
                });
            }
        }
        public async Task<Result<Unit, ErrorResponse>> DeletePlayerTaskPlayInDbAsync
            (int id, CancellationToken cancellationToken, bool commit = true)
        {
            try
            {
                await _unitOfWork.QuestRepository.DeletePlayerTaskPlayAsync(id, cancellationToken);
                if (commit)
                    await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting PlayerTaskPlay");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when deleting PlayerTaskPlay."
                });
            }
        }
        public async Task<Result<Unit, ErrorResponse>> UpdatePlayerTaskPlayInDbAsync
            (PlayerTaskPlay playerTaskPlay, CancellationToken cancellationToken, bool commit = true)
        {
            try
            {
                await _unitOfWork.QuestRepository.UpdatePlayerTaskPlay(playerTaskPlay);
                if (commit)
                    await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating PlayerTaskPlay");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when updating PlayerTaskPlay."
                });
            }
        }
        public async Task<Result<Unit, ErrorResponse>> UpdatePlayerTaskPlaysInBdAsync
            (List<PlayerTaskPlay> playerTaskPlays, CancellationToken cancellationToken, bool commit = true)
        {
            try
            {
                await _unitOfWork.QuestRepository.UpdatePlayerTaskPlays(playerTaskPlays);
                if (commit)
                    await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating PlayerTaskPlay");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when updating PlayerTaskPlay."
                });
            }
        }

        public async Task<Result<Unit, ErrorResponse>> AddRewardAsync
            (Reward reward, CancellationToken cancellationToken, bool commit = true)
        {
            try
            {
                await _unitOfWork.QuestRepository.AddRewardAsync(reward, cancellationToken);
                if (commit)
                    await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when adding Reward");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when adding Reward."
                });
            }
        }
        public async Task<Result<Unit, ErrorResponse>> DeleteRewardAsync
            (int id, CancellationToken cancellationToken, bool commit = true)
        {
            try
            {
                await _unitOfWork.QuestRepository.DeleteRewardAsync(id, cancellationToken);
                if (commit)
                    await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting Reward");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when deleting Reward."
                });
            }
        }
        public async Task<Result<Unit, ErrorResponse>> UpdateRewardAsync
            (Reward reward, CancellationToken cancellationToken, bool commit = true)
        {
            try
            {
                await _unitOfWork.QuestRepository.UpdateReward(reward);
                if (commit)
                    await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating Reward");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when updating Reward."
                });
            }
        }

    }
}
