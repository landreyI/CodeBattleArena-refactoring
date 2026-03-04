using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Helpers;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.DBServices;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Services.Judge0;
using CodeBattleArena.Server.Services.Notifications.INotifications;
using CodeBattleArena.Server.Specifications;
using CodeBattleArena.Server.Specifications.PlayerSessionSpec;
using CodeBattleArena.Server.Specifications.QuestSpec;
using CodeBattleArena.Server.Untils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CodeBattleArena.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("api-policy")]
    public class PlayerSessionController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IPlayerService _playerService;
        private readonly IPlayerSessionService _playerSessionService;
        private readonly UserManager<Player> _userManager;
        private readonly ITaskService _taskService;
        private readonly Judge0Client _judge0Client;
        private readonly ISessionNotificationService _sessionNotificationService;
        private readonly IMapper _mapper;
        public PlayerSessionController(ISessionService sessionService, UserManager<Player> userManager,
            IMapper mapper, IPlayerSessionService playerSessionService, IPlayerService playerService,
            ITaskService taskService, ISessionNotificationService sessionNotificationService, Judge0Client judge0Client)
        {
            _sessionService = sessionService;
            _userManager = userManager;
            _mapper = mapper;
            _playerSessionService = playerSessionService;
            _playerService = playerService;
            _taskService = taskService;
            _sessionNotificationService = sessionNotificationService;
            _judge0Client = judge0Client;
        }

        [HttpGet("info-player-session")]
        public async Task<IActionResult> GetPlayerSessionInfo(string? playerId, int? sessionId, CancellationToken cancellationToken)
        {
            var authUserId = _userManager.GetUserId(User);
            var targetPlayerId = playerId ?? authUserId;

            // Получение сессии: активной или по ID
            Session? session;
            if (!sessionId.HasValue || string.IsNullOrEmpty(playerId))
            {
                session = await _playerSessionService.GetActiveSession(targetPlayerId, cancellationToken);
            }
            else
            {
                var accessCheck = await _sessionService.CanAccessSessionPlayersAsync(sessionId.Value, authUserId, cancellationToken);
                if (!accessCheck.IsSuccess)
                    return UnprocessableEntity(accessCheck.Failure);

                if (!accessCheck.Success)
                    return UnprocessableEntity(new ErrorResponse { Error = "You do not have sufficient rights to view this player." });

                session = await _sessionService.GetSessionInDbAsync(sessionId.Value, cancellationToken);
            }

            if (session == null)
                return NotFound(new ErrorResponse { Error = "Session not found." });

            // Ограничение: нельзя просматривать чужой код
            bool isViewingOtherPlayer = targetPlayerId != authUserId;
            bool isOpponentInSession = session.PlayerSessions.Any(p => p.IdPlayer == authUserId && !p.IsCompleted);
            if (isViewingOtherPlayer && isOpponentInSession)
            {
                return NotFound(new ErrorResponse { Error = "You can't view your opponent's code :)" });
            }

            var spec = Specification<PlayerSession>.Combine(
                new PlayerSessionDefaultIncludesSpec(),
                new PlayerSessionByIdSpec(session.IdSession, targetPlayerId)
            );

            var playerSession = await _playerSessionService.GetPlayerSessionAsync(spec, cancellationToken);
            if (playerSession == null)
                return NotFound(new ErrorResponse { Error = "Player Session not found." });

            return Ok(_mapper.Map<PlayerSessionDto>(playerSession));
        }

        [Authorize]
        [HttpPut("finish-task")]
        public async Task<IActionResult> FinistTask(CancellationToken cancellationToken)
        {
            var currentUserId = _userManager.GetUserId(User);
            var activeSession = await _playerSessionService.GetActiveSession(currentUserId, cancellationToken);
            if (activeSession == null)
                return NotFound(new ErrorResponse { Error = "Not found active session." });

            var resultFinish = await _playerSessionService.FinishTask(activeSession.IdSession, currentUserId, cancellationToken);
            if (!resultFinish.IsSuccess)
                return UnprocessableEntity(resultFinish.Failure);

            var resultCount = await _sessionService.GetCountCompletedTaskAsync(activeSession.IdSession, cancellationToken);
            if (resultCount.IsSuccess)
                await _sessionNotificationService.NotifyUpdateCompletedCount(activeSession.IdSession, resultCount.Success);

            return Ok(true);
        }

        [Authorize]
        [HttpDelete("leave-session")]
        public async Task<IActionResult> LeaveSession(CancellationToken cancellationToken)
        {
            var currentUserId = _userManager.GetUserId(User);

            var activeSession = await _playerSessionService.GetActiveSession(currentUserId, cancellationToken);
            if (activeSession == null)
                return Ok(false);

            var resultDeleting = await _playerSessionService.DelPlayerSessionInDbAsync(activeSession.IdSession, currentUserId, cancellationToken);
            if (!resultDeleting.IsSuccess)
                return UnprocessableEntity(resultDeleting.Failure);

            var player = await _playerService.GetPlayerAsync(currentUserId, cancellationToken);
            var dtoPlayer = _mapper.Map<PlayerDto>(player);
            await _sessionNotificationService.NotifySessionUnjoinAsync(activeSession.IdSession, dtoPlayer);

            return Ok(true);
        }

        [Authorize]
        [HttpDelete("kick-out-session")]
        public async Task<IActionResult> KickOutSession(string idDeletePlayer, int idSession, CancellationToken cancellationToken)
        {
            var currentUserId = _userManager.GetUserId(User);

            var resultKick = await _playerSessionService.KickOutSessionAsync
                (currentUserId, idSession, idDeletePlayer, cancellationToken);

            if (!resultKick.IsSuccess)
                return UnprocessableEntity(resultKick.Failure);

            var player = await _playerService.GetPlayerAsync(currentUserId, cancellationToken);
            var dtoPlayer = _mapper.Map<PlayerDto>(player);
            await _sessionNotificationService.NotifySessionKickOutAsync(idSession, dtoPlayer);

            return Ok(true);
        }

        [Authorize]
        [HttpPut("join-session")]
        public async Task<IActionResult> JoinSession(int? idSession, string? password, CancellationToken cancellationToken)
        {
            if (idSession == null) return BadRequest(new ErrorResponse { Error = "Session ID not specified." });

            var resultCheck = await _sessionService.CheckPasswordAsync(password, idSession.Value, cancellationToken);
            if (!resultCheck.IsSuccess)
                return UnprocessableEntity(resultCheck.Failure);
            if (!resultCheck.Success)
                return UnprocessableEntity(new ErrorResponse { Error = "Wrong password." });

            var currentUserId = _userManager.GetUserId(User);
            var resultCreatePlayerSession = await _playerSessionService.CreatPlayerSession(currentUserId, idSession.Value, cancellationToken);
            if (!resultCreatePlayerSession.IsSuccess)
                return UnprocessableEntity(resultCreatePlayerSession.Failure);

            var player = await _playerService.GetPlayerAsync(currentUserId, cancellationToken);
            var dtoPlayer = _mapper.Map<PlayerDto>(player);

            await _sessionNotificationService.NotifySessionJoinAsync(idSession.Value, dtoPlayer);

            return Ok(true);
        }


        [HttpGet("update-code-player")]
        [EnableRateLimiting("none")]
        public async Task<IActionResult> UpdateCodePlayer(int? sessionId, string? code, CancellationToken cancellationToken)
        {
            await _sessionNotificationService.NotifyUpdateCodePlayerAsync(sessionId.Value, code);
            return Ok(true);
        }

        [Authorize]
        [HttpGet("send-message-session")]
        public async Task<IActionResult> SendMessageSession(string? message, CancellationToken cancellationToken)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var activeSession = await _playerSessionService.GetActiveSession(currentUser.Id, cancellationToken);
            if (activeSession == null || string.IsNullOrEmpty(message))
                return Ok(false);

            var messageDto = new MessageDto
            {
                IdSender = currentUser.Id,
                Sender = _mapper.Map<PlayerDto>(currentUser),
                MessageText = message,
            };

            await _sessionNotificationService.NotifySendMessageSessionAsync(activeSession.IdSession, messageDto);
            return Ok(true);
        }

        [Authorize]
        [HttpPost("check-code-player")]
        public async Task<IActionResult> CheckCodePlayer([FromBody] CodeRequest codeRequest, CancellationToken cancellationToken)
        {
            var currentUserId = _userManager.GetUserId(User);

            var activeSession = await _playerSessionService.GetActiveSession(currentUserId, cancellationToken);
            if (activeSession == null)
                return NotFound(new ErrorResponse { Error = "Not found active session." });

            var inputDataList = await _taskService.GetTaskInputDataByIdTaskProgrammingAsync(
                activeSession.TaskId!.Value,
                cancellationToken);

            var payload = CodeCheckBuilder.Build(codeRequest.Code, activeSession.TaskProgramming, inputDataList);

            var result = await _judge0Client.CheckAsync(
                payload.source_code,
                activeSession.LangProgramming.IdCheckApi,
                payload.stdin,
                payload.expected_output);

            var resultSave = await _playerSessionService.SaveCheckCodeAsync
                (activeSession.IdSession, currentUserId, codeRequest.Code ,result, cancellationToken);

            if (!resultSave.IsSuccess)
                return UnprocessableEntity(resultSave.Failure);

            await _sessionNotificationService.NotifyUpdatePlayerSessionAsync
                (_mapper.Map<PlayerSessionDto>(resultSave.Success));

            return Ok(result);
        }

        [HttpGet("player-sessions")]
        public async Task<IActionResult> GetPlayerSessions(string id, CancellationToken cancellationToken)
        {
            var spec = Specification<PlayerSession>.Combine(
                new PlayerSessionDefaultIncludesSpec(),
                new PlayerSessionByIdSpec(idPlayer: id)
            );
            var playerSessions = await _playerSessionService.GetListPlayerSessionAsync(spec, cancellationToken);

            var playerSessionsDto = _mapper.Map<List<PlayerSessionDto>>(playerSessions);

            return Ok(playerSessionsDto.Select(S => S.Session));
        }
    }
}
