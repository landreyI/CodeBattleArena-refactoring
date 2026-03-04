using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Helpers;
using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Services.Judge0;
using CodeBattleArena.Server.Services.Notifications.INotifications;
using CodeBattleArena.Server.Specifications;
using CodeBattleArena.Server.Specifications.PlayerSessionSpec;
using CodeBattleArena.Server.Specifications.SessionSpec;
using CodeBattleArena.Server.Untils;
using System.Threading;

namespace CodeBattleArena.Server.Services.DBServices
{
    public class PlayerSessionService : IPlayerSessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PlayerSessionService> _logger;
        private readonly ISessionService _sessionService;
        private readonly IPlayerService _playerService;
        private readonly IMapper _mapper;
        private readonly IPlayerNotificationService _playerNotificationService;

        public PlayerSessionService(IUnitOfWork unitOfWork, ILogger<PlayerSessionService> logger,
            ISessionService sessionService, IPlayerService playerService, IMapper mapper,
            IPlayerNotificationService playerNotificationService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _sessionService = sessionService;
            _playerService = playerService;
            _mapper = mapper;
            _playerNotificationService = playerNotificationService;
        }

        public async Task<Result<PlayerSession, ErrorResponse>> CreatPlayerSession
            (string idPlayer, int idSession, CancellationToken ct, bool commit = true)
        {
            PlayerSession playerSession = new PlayerSession
            {
                IdPlayer = idPlayer,
                IdSession = idSession,
            };

            try
            {
                var sessions = await GetListPlayerSessionAsync(new PlayerSessionByIdSpec(idPlayer: idPlayer), ct);
                bool isActive = sessions.Any(s => !s.Session.IsFinish);
                if (isActive)
                    return Result.Failure<PlayerSession, ErrorResponse>(new ErrorResponse 
                    { 
                        Error= "You already have an active session." 
                    });

                var session = await _sessionService.GetSessionAsync(new SessionWithPlayersSpec(idSession), ct);
                if(session == null)
                    return Result.Failure<PlayerSession, ErrorResponse>(new ErrorResponse 
                    { 
                        Error="Session not found." 
                    });

                if (BusinessRules.IsStartetSession(session))
                    return Result.Failure<PlayerSession, ErrorResponse>(new ErrorResponse
                    {
                        Error = "The session owner has already started the game."
                    });

                if (session.MaxPeople == session.PlayerSessions.Count)
                    return Result.Failure<PlayerSession, ErrorResponse>(new ErrorResponse
                    {
                        Error = "This session has the maximum number of participants."
                    });

                var addResult = await AddPlayerSessionInDbAsync(playerSession, ct, commit);
                if(!addResult.IsSuccess)
                    return Result.Failure<PlayerSession, ErrorResponse>(addResult.Failure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in CreatePlayerSession");
                return Result.Failure<PlayerSession, ErrorResponse>(new ErrorResponse 
                { 
                    Error = "Unexpected server error." 
                });
            }

            return Result.Success<PlayerSession, ErrorResponse>(playerSession);
        }

        public async Task<Result<Unit, ErrorResponse>> KickOutSessionAsync
            (string idAuthPlayer, int idSession, string idDeletePlayer, CancellationToken ct, bool commit = true)
        {
            var activeSession = await GetActiveSession(idDeletePlayer, ct);
            if (activeSession == null)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Active session not found."
                });

            var roles = await _playerService.GetRolesAsync(idAuthPlayer);
            var isEditSession = BusinessRules.IsEditSession(idAuthPlayer, activeSession, roles);

            if(!isEditSession)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "You do not have permission to perform this action."
                });

            var resultDeleting = await DelPlayerSessionInDbAsync(idSession, idDeletePlayer, ct, commit);
            if (!resultDeleting.IsSuccess)
                return resultDeleting;

            return Result.Success<Unit, ErrorResponse>(Unit.Value);
        }
        public async Task<Session> GetActiveSession(string idPlayer, CancellationToken ct)
        {
            var checkResult = ValidationHelper.CheckUserId<bool>(idPlayer);
            if (!checkResult.IsSuccess)
                return null;
            var spec = Specification<PlayerSession>.Combine(
                new PlayerSessionDefaultIncludesSpec(),
                new PlayerSessionByIdSpec(idPlayer: idPlayer)
            );
            var sessions = await GetListPlayerSessionAsync(spec, ct);
            var activeSessionId = sessions
                .FirstOrDefault(s => !s.Session.IsFinish)?.IdSession;

            if (activeSessionId == null)
                return null;
            var session = await _sessionService.GetSessionAsync(new SessionWithPlayersSpec(activeSessionId.Value), ct);
            return session;
        }
        public async Task<Result<PlayerSession, ErrorResponse>> SaveCheckCodeAsync
            (int idSession, string idPlayer, string code, ExecutionResult executionResult, CancellationToken ct, bool commit = true)
        {
            var playerSession = await GetPlayerSessionAsync(new PlayerSessionByIdSpec(idSession, idPlayer), ct);

            if(playerSession.IsCompleted)
                return Result.Failure<PlayerSession, ErrorResponse>(new ErrorResponse 
                { 
                    Error = "You have already completed the task"
                });

            playerSession.Memory = executionResult.Memory;
            playerSession.Time = executionResult.Time;
            playerSession.CodeText = code;

            var resultUpdate = await UpdatePlayerSessionInDbAsync(playerSession, ct, commit);
            if (!resultUpdate.IsSuccess)
                return Result.Failure<PlayerSession, ErrorResponse>(resultUpdate.Failure);

            return Result.Success<PlayerSession, ErrorResponse>(playerSession);
        }

        public async Task<Result<Unit, ErrorResponse>> InviteSessionAsync
            (string authUserId, List<string> idPlayersInvite, CancellationToken ct)
        {
            var activeSession = await GetActiveSession(authUserId, ct);
            if (activeSession == null)
                return Result.Failure<Unit, ErrorResponse>
                    (new ErrorResponse { Error = "Not found active session." });

            var resultIsEdit = await _sessionService.CanEditSessionAsync(activeSession.IdSession, authUserId, ct);
            if (!resultIsEdit.IsSuccess)
                return Result.Failure<Unit, ErrorResponse>(resultIsEdit.Failure);

            bool isEdit = resultIsEdit.Success;
            if (!isEdit)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "You do not have sufficient permissions to edit this session."
                });

            foreach (var idPlayer in idPlayersInvite)
            {
                await _playerNotificationService.NotifyInvitationSessionAsync(idPlayer, _mapper.Map<SessionDto>(activeSession));
            }

            return Result.Success<Unit, ErrorResponse>(Unit.Value);
        }

        public async Task<Result<Unit, ErrorResponse>> FinishTask(int idSession, string idPlayer, CancellationToken ct, bool commit = true)
        {
            var playerSession = await GetPlayerSessionAsync(new PlayerSessionByIdSpec(idSession, idPlayer), ct);

            if (playerSession.IsCompleted)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "You have already completed the task"
                });

            var resultFinish = await FinishTaskInDbAsync(idSession, idPlayer, ct, commit);
            if (!resultFinish.IsSuccess)
                return Result.Failure<Unit, ErrorResponse>(resultFinish.Failure);

            return Result.Success<Unit, ErrorResponse>(Unit.Value);
        }

        public async Task<Result<Unit, ErrorResponse>> AddPlayerSessionInDbAsync
            (PlayerSession playerSession, CancellationToken ct, bool commit = true)
        {
            try
            {
                await _unitOfWork.PlayerSessionRepository.AddPlayerSessionAsync(playerSession, ct);
                if (commit)
                    await _unitOfWork.CommitAsync(ct);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding PlayerSession");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse { Error = "Database error when adding playerSession." });
            }
        }        
        public async Task<PlayerSession> GetPlayerSessionAsync(ISpecification<PlayerSession> spec, CancellationToken ct)
        {
            return await _unitOfWork.PlayerSessionRepository.GetPlayerSessionAsync(spec, ct);
        }
        public async Task<List<PlayerSession>> GetListPlayerSessionAsync(ISpecification<PlayerSession> spec, CancellationToken ct)
        {
            return await _unitOfWork.PlayerSessionRepository.GetListPlayerSessionByIdAsync(spec, ct);
        }

        public async Task<Result<Unit, ErrorResponse>> UpdatePlayerSessionInDbAsync
            (PlayerSession playerSession, CancellationToken ct, bool commit = true)
        {
            try
            {
                await _unitOfWork.PlayerSessionRepository.UpdatePlayerSession(playerSession);
                if (commit)
                    await _unitOfWork.CommitAsync(ct);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating PlayerSession");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse { Error = "Database error when finish task playerSession." });
            }
        }
        public async Task<Result<Unit, ErrorResponse>> FinishTaskInDbAsync
            (int idSession, string idPlayer, CancellationToken ct, bool commit = true)
        {
            try
            {
                await _unitOfWork.PlayerSessionRepository.FinishTaskAsync(idSession, idPlayer, ct);
                if (commit)
                    await _unitOfWork.CommitAsync(ct);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error FinishTask");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse { Error = "Database error when finish task playerSession." });

            }
        }
        public async Task<Result<Unit, ErrorResponse>> DelPlayerSessionInDbAsync
            (int idSession, string idPlayer, CancellationToken ct, bool commit = true)
        {
            try
            {
                await _unitOfWork.PlayerSessionRepository.DelPlayerSessionAsync(idSession, idPlayer, ct);
                if (commit)
                    await _unitOfWork.CommitAsync(ct);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting PlayerSession");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when delete playerSession."
                });
            }
        }
    }
}
