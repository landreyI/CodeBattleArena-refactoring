using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Helpers;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.DBServices;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Services.Notifications.INotifications;
using CodeBattleArena.Server.Untils;
using Google.Apis.Gmail.v1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService _friendService;
        private readonly UserManager<Player> _userManager;
        private readonly IMapper _mapper;
        private readonly IPlayerNotificationService _playerNotificationService;

        public FriendController(UserManager<Player> userManager, IFriendService friendService, 
            IMapper mapper, IPlayerNotificationService playerNotificationService)
        {
            _userManager = userManager;
            _friendService = friendService;
            _mapper = mapper;
            _playerNotificationService = playerNotificationService;
        }

        [Authorize]
        [HttpGet("list-friends")]
        public async Task<IActionResult> GetListFriends(CancellationToken cancellationToken)
        {
            var authUserId = _userManager.GetUserId(User);
            var friends = await _friendService.GetAllFriendsAsync(authUserId, cancellationToken);
            return Ok(_mapper.Map<List<FriendDto>>(friends));
        }

        [Authorize]
        [HttpGet("friendship-friends")]
        public async Task<IActionResult> GetFriendshipFriends(CancellationToken cancellationToken)
        {
            var authUserId = _userManager.GetUserId(User);
            var friends = await _friendService.GetFriendshipFriends(authUserId, cancellationToken);
            return Ok(_mapper.Map<List<FriendDto>>(friends));
        }

        [Authorize]
        [HttpPost("add-friend")]
        public async Task<IActionResult> AddFriend([FromBody] string? addresseeId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(addresseeId)) return BadRequest(new ErrorResponse { Error = "Player ID not specified." });
            var authUserId = _userManager.GetUserId(User);

            var resultAdd = await _friendService.AddFriendAsync(authUserId, addresseeId, cancellationToken);
            if(!resultAdd.IsSuccess)
                return UnprocessableEntity(resultAdd.Failure);

            var playerSender = await _userManager.GetUserAsync(User);
            var dto = _mapper.Map<PlayerDto>(playerSender);

            await _playerNotificationService.NotifyFriendRequestAsync(addresseeId, dto);

            return Ok(true);
        }

        [Authorize]
        [HttpPut("approve-friendship")]
        public async Task<IActionResult> ApproveFriendship([FromBody] int? idFriend, CancellationToken cancellationToken)
        {
            if (!idFriend.HasValue) return BadRequest(new ErrorResponse { Error = "Friend ID not specified." });
            var authUserId = _userManager.GetUserId(User);

            var resultApprove = await _friendService.ApproveFriendshipAsync(authUserId, idFriend.Value, cancellationToken);
            if (!resultApprove.IsSuccess)
                return UnprocessableEntity(resultApprove.Failure);

            return Ok(true);
        }
        [Authorize]
        [HttpDelete("delete-friend")]
        public async Task<IActionResult> DeleteFriend(int? idFriend, CancellationToken cancellationToken)
        {
            if (!idFriend.HasValue) return BadRequest(new ErrorResponse { Error = "Friend ID not specified." });
            var authUserId = _userManager.GetUserId(User);

            var resultDelete = await _friendService.DeleteFriendAsync(authUserId, idFriend.Value, cancellationToken);
            if (!resultDelete.IsSuccess)
                return UnprocessableEntity(resultDelete.Failure);

            return Ok(true);
        }
    }
}
