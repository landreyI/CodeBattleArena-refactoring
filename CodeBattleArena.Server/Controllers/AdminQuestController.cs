using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Helpers;
using CodeBattleArena.Server.Infrastructure.Attributes;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Untils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Server.Controllers
{
    [ApiController]
    [Authorize]
    [RequireEditRole]
    [Route("api/[controller]")]
    public class AdminQuestController : Controller
    {
        private readonly IQuestService _questService;
        private readonly IMapper _mapper;
        public AdminQuestController(IQuestService questService, IMapper mapper)
        {
            _questService = questService;
            _mapper = mapper;
        }

        [HttpPost("create-task-play")]
        public async Task<IActionResult> CreateTaskPlay
            ([FromBody] CreateTaskPlayRequest createTaskPlayRequest, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return UnprocessableEntity(errors);
            }

            var taskPlay = _mapper.Map<TaskPlay>(createTaskPlayRequest.TaskPlay);
            var resultCreate = await _questService.AddTaskPlayAsync(taskPlay, createTaskPlayRequest.IdRewards, cancellationToken);
            if (!resultCreate.IsSuccess)
                return UnprocessableEntity(resultCreate.Failure);

            return Ok(taskPlay.IdTask);
        }

        [HttpDelete("delete-task-play")]
        public async Task<IActionResult> DeleteTaskPlay(int? id, CancellationToken cancellationToken)
        {
            if (id == null) return BadRequest(new ErrorResponse { Error = "TaskPlay ID not specified." });

            var resultDeleting = await _questService.DeleteTaskPlayAsync(id.Value, cancellationToken);
            if (!resultDeleting.IsSuccess)
                return UnprocessableEntity(resultDeleting.Failure);

            return Ok(true);
        }

        [HttpPut("edit-task-play")]
        public async Task<IActionResult> EditTaskPlay([FromBody] CreateTaskPlayRequest updateTaskPlayRequest, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return UnprocessableEntity(errors);
            }

            var resultUpdate = await _questService.UpdateTaskPlayAsync(updateTaskPlayRequest.TaskPlay, updateTaskPlayRequest.IdRewards, cancellationToken);

            if (!resultUpdate.IsSuccess)
                return UnprocessableEntity(resultUpdate.Failure);

            return Ok(true);
        }

        [HttpPost("create-reward")]
        public async Task<IActionResult> CreateReward([FromBody] RewardDto rewardDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return UnprocessableEntity(errors);
            }

            var reward = _mapper.Map<Reward>(rewardDto);
            var resultCreate = await _questService.AddRewardAsync(reward, cancellationToken);
            if (!resultCreate.IsSuccess)
                return UnprocessableEntity(resultCreate.Failure);

            return Ok(reward.IdReward);
        }

        [HttpDelete("delete-reward")]
        public async Task<IActionResult> DeleteReward(int? id, CancellationToken cancellationToken)
        {
            if (id == null) return BadRequest(new ErrorResponse { Error = "Reward ID not specified." });

            var resultDeleting = await _questService.DeleteRewardAsync(id.Value, cancellationToken);
            if (!resultDeleting.IsSuccess)
                return UnprocessableEntity(resultDeleting.Failure);

            return Ok(true);
        }

        [HttpPut("edit-reward")]
        public async Task<IActionResult> EditReward([FromBody] RewardDto rewardDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return UnprocessableEntity(errors);
            }

            var resultUpdate = await _questService.UpdateRewardAsync(_mapper.Map<Reward>(rewardDto), cancellationToken);
            if (!resultUpdate.IsSuccess)
                return UnprocessableEntity(resultUpdate.Failure);

            return Ok(true);
        }
    }
}
