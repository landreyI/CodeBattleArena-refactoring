using AutoMapper;
using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.DBServices;
using CodeBattleArena.Server.Services.Notifications.INotifications;
using CodeBattleArena.Server.Specifications.PlayerSessionSpec;
using CodeBattleArena.Server.Specifications;
using CodeBattleArena.Server.Specifications.SessionSpec;
using CodeBattleArena.Server.Untils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Services.DBServices.IDBServices;

namespace CodeBattleArena.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IPlayerService _playerService;
        private readonly IPlayerSessionService _playerSessionService;
        private readonly UserManager<Player> _userManager;
        private readonly ISessionNotificationService _sessionNotificationService;
        private readonly IMapper _mapper;
        public SessionController(ISessionService sessionService, UserManager<Player> userManager,
            IMapper mapper, IPlayerSessionService playerSessionService, IPlayerService playerService,
            ISessionNotificationService sessionNotificationService)
        {
            _sessionService = sessionService;
            _userManager = userManager;
            _mapper = mapper;
            _playerSessionService = playerSessionService;
            _playerService = playerService;
            _sessionNotificationService = sessionNotificationService;
        }

        [Authorize]
        [HttpPut("start-game")]
        public async Task<IActionResult> StartGame(int? idSession, CancellationToken cancellationToken)
        {
            if (idSession == null) return BadRequest(new ErrorResponse { Error = "Session ID not specified." });

            var currentUserId = _userManager.GetUserId(User);
            var resultStart = await _sessionService.StartGameAsync(idSession.Value, currentUserId, cancellationToken);
            if (!resultStart.IsSuccess)
                return UnprocessableEntity(resultStart.Failure);

            var session = await _sessionService.GetSessionInDbAsync(idSession.Value, cancellationToken);
            var dto = _mapper.Map<SessionDto>(session);

            await _sessionNotificationService.NotifyStartGameAsync(idSession.Value);
            await _sessionNotificationService.NotifySessionUpdatedGroupAsync(dto);
            await _sessionNotificationService.NotifySessionUpdatedAllAsync(dto);

            return Ok(true);
        }

        [Authorize]
        [HttpPut("finish-game")]
        public async Task<IActionResult> FinishGame(int? idSession, CancellationToken cancellationToken)
        {
            if (idSession == null) return BadRequest(new ErrorResponse { Error = "Session ID not specified." });

            var currentUserId = _userManager.GetUserId(User);
            var resultFinish = await _sessionService.FinishGameAsync(idSession.Value, currentUserId, cancellationToken);
            if (!resultFinish.IsSuccess)
                return UnprocessableEntity(resultFinish.Failure);

            var session = await _sessionService.GetSessionInDbAsync(idSession.Value, cancellationToken);
            var dto = _mapper.Map<SessionDto>(session);

            await _sessionNotificationService.NotifyFinishGameAsync(idSession.Value);

            return Ok(true);
        }

        [HttpGet("best-result-session")]
        public async Task<IActionResult> GetBestResultSession(int? idSession, CancellationToken cancellationToken)
        {
            if (idSession == null) return BadRequest(new ErrorResponse { Error = "Session ID not specified." });
            var result = await _sessionService.GetVinnerAsync(idSession.Value, cancellationToken);
            return Ok(_mapper.Map<PlayerSessionDto>(result));
        }

        [Authorize]
        [HttpGet("active-session")]
        public async Task<IActionResult> GetActiveSession(CancellationToken cancellationToken)
        {
            var currentUserId = _userManager.GetUserId(User);

            var activeSession = await _playerSessionService.GetActiveSession(currentUserId, cancellationToken);
            return Ok(_mapper.Map<SessionDto>(activeSession));
        }

        [HttpGet("count-completed-task")]
        public async Task<IActionResult> GetCountCompletedTask(int idSession, CancellationToken cancellationToken)
        {
            var resultCount = await _sessionService.GetCountCompletedTaskAsync(idSession, cancellationToken);
            if(!resultCount.IsSuccess)
                return UnprocessableEntity(resultCount.Failure);

            return Ok(resultCount.Success);
        }

        [HttpGet("info-session")]
        public async Task<IActionResult> GetSession(int? id, CancellationToken cancellationToken)
        {
            if (id == null) return BadRequest(new ErrorResponse { Error = "Session ID not specified." });

            var session = await _sessionService.GetSessionInDbAsync(id.Value, cancellationToken);
            if (session == null) return NotFound(new ErrorResponse { Error = "Session not found." });

            SessionDto sessionDto = new SessionDto();
            _mapper.Map(session, sessionDto);

            var authUserId = _userManager.GetUserId(User);

            var result = await _sessionService.CanEditSessionAsync(sessionDto.IdSession.Value, authUserId, cancellationToken);
            if(!result.IsSuccess)
                return UnprocessableEntity(result.Failure);

            bool isEdit = result.Success;

            return Ok(new { session = sessionDto, isEdit = isEdit });
        }

        [HttpGet("list-sessions")]
        public async Task<IActionResult> GetSessionsList([FromQuery] SessionFilter? filter, CancellationToken cancellationToken)
        {
            var sessions = await _sessionService.GetListSessionAsync(filter, cancellationToken);
            return Ok(_mapper.Map<List<SessionDto>>(sessions));
        }

        [HttpGet("session-players")]
        public async Task<IActionResult> GetSessionPlayers(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                return BadRequest(new ErrorResponse { Error = "Session ID not specified." });
            }

            var currentUserId = _userManager.GetUserId(User);

            var result = await _sessionService.CanAccessSessionPlayersAsync(id.Value, currentUserId, cancellationToken); 

            if (!result.IsSuccess)
                return UnprocessableEntity(result.Failure);

            if(!result.Success)
                return NoContent();

            var spec = Specification<PlayerSession>.Combine(
                new PlayerSessionDefaultIncludesSpec(),
                new PlayerSessionByIdSpec(id.Value)
            );
            var playerSessions = await _playerSessionService.GetListPlayerSessionAsync(spec, cancellationToken);

            var playerSessionsDto = _mapper.Map<List<PlayerSessionDto>>(playerSessions);

            return Ok(playerSessionsDto.Select(S => S.Player));
        }

        [Authorize]
        [HttpGet("invite-session")]
        public async Task<IActionResult> InviteSession([FromQuery] List<string>? idPlayersInvite, CancellationToken cancellationToken)
        {
            if (idPlayersInvite == null || idPlayersInvite.Count == 0)
                return BadRequest(new ErrorResponse { Error = "No IDs provided." });

            var authUserId = _userManager.GetUserId(User);

            var resultInvite = await _playerSessionService.InviteSessionAsync(authUserId, idPlayersInvite, cancellationToken);
            if(!resultInvite.IsSuccess)
                return UnprocessableEntity(resultInvite.Failure);

            return Ok(true);
        }

        [Authorize]
        [HttpGet("select-task-for-session")]
        public async Task<IActionResult> SelectTaskForSession(int? sessionId, int? taskId, CancellationToken cancellationToken)
        {
            if (sessionId == null) return BadRequest(new ErrorResponse { Error = "Session ID not specified." });
            if (taskId == null) return BadRequest(new ErrorResponse { Error = "Task ID not specified." });

            var currentUserId = _userManager.GetUserId(User);

            var resultSelect = await _sessionService.SelectTaskForSessionAsync(
                currentUserId, sessionId.Value, taskId.Value, cancellationToken
                );

            if (!resultSelect.IsSuccess)
                return UnprocessableEntity(resultSelect.Failure);

            await _sessionNotificationService.NotifySessionUpdatedGroupAsync(resultSelect.Success);
            await _sessionNotificationService.NotifySessionUpdatedAllAsync(resultSelect.Success);

            return Ok(true);
        }

        [Authorize]
        [HttpDelete("delete-session")]
        public async Task<IActionResult> DeleteSession(int? id, CancellationToken cancellationToken)
        {
            if (id == null) return BadRequest(new ErrorResponse { Error = "Session ID not specified." });
            var currentUserId = _userManager.GetUserId(User);

            var resultDeleting = await _sessionService.DeletingSessionAsync(currentUserId, id.Value, cancellationToken);
            if (!resultDeleting.IsSuccess)
                return UnprocessableEntity(resultDeleting.Failure);

            await _sessionNotificationService.NotifySessionDeletedGroupAsync(id.Value);
            await _sessionNotificationService.NotifySessionDeletedAllAsync(id.Value);

            return Ok(true);
        }

        [Authorize]
        [HttpPost("create-session")]
        public async Task<IActionResult> CreateSession([FromBody] SessionDto sessionDto, CancellationToken cancellationToken)
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

            var currentUserId = _userManager.GetUserId(User);

            var resultCreateSession = await _sessionService.CreateSessionAsync(currentUserId, sessionDto, cancellationToken);
            if (!resultCreateSession.IsSuccess)
                return UnprocessableEntity(resultCreateSession.Failure);

            Session session = resultCreateSession.Success;

            var resultCreatePlayerSession = await _playerSessionService.CreatPlayerSession(currentUserId, session.IdSession, cancellationToken);
            if(!resultCreatePlayerSession.IsSuccess)
                return UnprocessableEntity(resultCreatePlayerSession.Failure);

            await _sessionNotificationService.NotifySessionAddAsync(_mapper.Map(session, new SessionDto()));

            return Ok(session.IdSession);
        }

        [Authorize]
        [HttpPut("edit-session")]
        public async Task<IActionResult> EditSession([FromBody] SessionDto sessionDto, CancellationToken cancellationToken)
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

            var resultUpdate = await _sessionService.UpdateSessionAsync(authUserId, sessionDto, cancellationToken);
            if (!resultUpdate.IsSuccess)
                return UnprocessableEntity(resultUpdate.Failure);

            var session = await _sessionService.GetSessionAsync(new SessionByIdSpec(sessionDto.IdSession.Value), cancellationToken);
            var dto = _mapper.Map<SessionDto>(session);

            await _sessionNotificationService.NotifySessionUpdatedGroupAsync(dto);
            await _sessionNotificationService.NotifySessionUpdatedAllAsync(dto);

            return Ok(true);
        }
    }
}
