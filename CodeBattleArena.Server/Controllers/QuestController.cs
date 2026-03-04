using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Helpers;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Specifications;
using CodeBattleArena.Server.Specifications.QuestSpec;
using CodeBattleArena.Server.Specifications.SessionSpec;
using CodeBattleArena.Server.Untils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestController : Controller
    {
        private readonly IQuestService _questService;
        private readonly IMapper _mapper;
        public QuestController(IMapper mapper, IQuestService questService)
        {
            _mapper = mapper;
            _questService = questService;
        }

        [HttpGet("info-quest")]
        public async Task<IActionResult> GetQuest(int id, CancellationToken cancellationToken)
        {
            var taskPlay = await _questService.GetTaskPlayAsync(new TaskPlaySpec(id), cancellationToken);
            return Ok(_mapper.Map<TaskPlayDto>(taskPlay));
        }

        [HttpGet("list-quests")]
        public async Task<IActionResult> GetQuests(CancellationToken cancellationToken)
        {
            var tasksPlay = await _questService.GetListTaskPlayAsync(new TaskPlayDefaultIncludesSpec(), cancellationToken);
            return Ok(_mapper.Map<List<TaskPlayDto>>(tasksPlay));
        }

        [HttpGet("list-player-progress")]
        public async Task<IActionResult> GetListPlayerProgress(string? idPlayer, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(idPlayer)) return BadRequest
                    (new ErrorResponse { Error = "idPlayer ID not specified." });

            var listPlayerTaskPlay = await _questService
                .GetListPlayerTaskPlayAsync(new PlayerTaskPlaySpec(idPlayer: idPlayer), cancellationToken);

            return Ok(_mapper.Map<List<PlayerTaskPlayDto>>(listPlayerTaskPlay));
        }

        [HttpGet("player-progress")]
        public async Task<IActionResult> GetPlayerProgress(string? idPlayer, int? idTaskPlay, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(idPlayer)) return BadRequest
                    (new ErrorResponse { Error = "idPlayer ID not specified." });
            if (!idTaskPlay.HasValue) return BadRequest
                    (new ErrorResponse { Error = "idTaskPlay ID not specified." });

            var spec = Specification<PlayerTaskPlay>.Combine(
                new PlayerTaskPlaySpec(idTaskPlay.Value, idPlayer)
            );
            var playerTaskPlay = await _questService.GetPlayerTaskPlayAsync(spec, cancellationToken);

            return Ok(_mapper.Map<PlayerTaskPlayDto>(playerTaskPlay));
        }

        [HttpGet("list-rewards")]
        public async Task<IActionResult> GetRewards(CancellationToken cancellationToken) {
            var rewards = await _questService.GetRewardsAsync(cancellationToken);
            return Ok(_mapper.Map<List<RewardDto>>(rewards));
        }

        [HttpGet("list-task-play-rewards")]
        public async Task<IActionResult> GetTaskPlayRewards(int? id, CancellationToken cancellationToken)
        {
            if (id == null) return Ok();

            var taskPlayRewards = await _questService.GetTaskPlayRewardsAsync(id.Value, cancellationToken);
            var rewards = taskPlayRewards.Select(x => x.Reward).ToList();
            return Ok(_mapper.Map<List<RewardDto>>(rewards));
        }

        [Authorize]
        [HttpPut("claim-reward")]
        public async Task<IActionResult> ClaimReward([FromBody] ClaimRewardRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.IdPlayer)) return BadRequest
                (new ErrorResponse { Error = "idPlayer ID not specified." });
            if (!request.IdTaskPlay.HasValue) return BadRequest
                (new ErrorResponse { Error = "idTask ID not specified." });

            var resultClaim = await _questService.ClaimRewardAsync(request.IdPlayer, request.IdTaskPlay.Value, cancellationToken);
            if(!resultClaim.IsSuccess)
                return UnprocessableEntity(resultClaim.Failure);

            return Ok(true);
        }
    }
}
