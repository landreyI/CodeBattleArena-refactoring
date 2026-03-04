using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.Helpers;
using CodeBattleArena.Server.Infrastructure.Attributes;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.DBServices;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Untils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CodeBattleArena.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : Controller
    {
        private readonly IPlayerService _playerService;
        private readonly IPlayerSessionService _playerSessionService;
        private readonly UserManager<Player> _userManager;
        private readonly IMapper _mapper;
        public PlayerController(UserManager<Player> userManager, IPlayerService playerService, IMapper mapper, IPlayerSessionService playerSessionService)
        {
            _userManager = userManager;
            _playerService = playerService;
            _mapper = mapper;
            _playerSessionService = playerSessionService;
        }

        [HttpGet("info-player")]
        public async Task<IActionResult> GetPlayer(string? id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest(new ErrorResponse { Error = "Player ID not specified." });

            var authUserId = _userManager.GetUserId(User);

            var resultInfo = await _playerService.GetPlayerInfoAsync(id, authUserId, cancellationToken);

            if(!resultInfo.IsSuccess)
                return UnprocessableEntity(resultInfo.Failure);

            return Ok(new { player = resultInfo.Success.Player, isEdit = resultInfo.Success.IsEdit });
        }

        [HttpGet("list-players")]
        public async Task<IActionResult> GetPlayers([FromQuery] PlayerFilter? filter, CancellationToken cancellationToken)
        {
            var players = await _playerService.GetPlayersAsync(filter, cancellationToken);
            var dtosPlayer = _mapper.Map<List<PlayerDto>>(players);

            return Ok(dtosPlayer);
        }

        [Authorize]
        [HttpPut("edit-player")]
        public async Task<IActionResult> EditPlayer([FromBody] PlayerDto playerDto, CancellationToken cancellationToken)
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

            var authUserId = _userManager.GetUserId(User);

            var result = await _playerService.UpdatePlayerAsync(authUserId, playerDto, cancellationToken);
            if(!result.IsSuccess)
                return UnprocessableEntity(result.Failure);

            return Ok(true);
        }

        [Authorize]
        [HttpPut("change-active-item")]
        public async Task<IActionResult> ChangeActiveItem(string? idPlayer, int? idItem, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(idPlayer)) return BadRequest(new ErrorResponse { Error = "Player ID not specified." });
            if (idItem == null) return BadRequest(new ErrorResponse { Error = "Item ID not specified." });

            var authUserId = _userManager.GetUserId(User);

            var resultChange = await _playerService.ChangeActiveItem(idPlayer, authUserId, idItem.Value, cancellationToken);
            if(!resultChange.IsSuccess)
                return UnprocessableEntity(resultChange.Failure);

            return Ok(true);
        }
    }
}
