using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.Helpers;
using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.QuestSystem;
using CodeBattleArena.Server.QuestSystem.Dispatcher;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Services.Notifications.INotifications;
using CodeBattleArena.Server.Specifications;
using CodeBattleArena.Server.Specifications.PlayerSessionSpec;
using CodeBattleArena.Server.Specifications.SessionSpec;
using CodeBattleArena.Server.Untils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Threading;
using static Azure.Core.HttpHeader;

namespace CodeBattleArena.Server.Services.DBServices
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SessionService> _logger;
        private readonly IMapper _mapper;
        private readonly IPlayerService _playerService;
        private readonly ISessionNotificationService _sessionNotificationService;
        private readonly GameEventDispatcher _gameEventDispatcher;
        public SessionService(IUnitOfWork unitOfWork, ILogger<SessionService> logger, IMapper mapper,
            IPlayerService playerService, ISessionNotificationService sessionNotificationService, 
            GameEventDispatcher gameEventDispatcher)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _playerService = playerService;
            _sessionNotificationService = sessionNotificationService;
            _gameEventDispatcher = gameEventDispatcher;
        }

        public async Task<Result<Unit, ErrorResponse>> StartGameAsync
            (int sessionId, string userId, CancellationToken ct, bool commit = true)
        {
            var resultIsEdit = await CanEditSessionAsync(sessionId, userId, ct);
            if (!resultIsEdit.IsSuccess)
                return Result.Failure<Unit, ErrorResponse>(resultIsEdit.Failure);

            bool isEdit = resultIsEdit.Success;
            if (!isEdit)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "You do not have sufficient permissions to edit this session."
                });

            var session = await GetSessionAsync(new SessionByIdSpec(sessionId), ct);
            if (session.IsStart || session.IsFinish)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "At this stage the session cannot be launched for the game."
                });

            var resultStart = await StartGameInDbAsync(sessionId, ct, commit);
            if(!resultStart.IsSuccess)
                return resultStart;

            return Result.Success<Unit, ErrorResponse>(Unit.Value);
        }
        public async Task<Result<Unit, ErrorResponse>> FinishGameAsync
            (int sessionId, string userId, CancellationToken ct, bool? isBackground = false) //isBackground - фоновое выполнение из сервера
        {
            var resultIsEdit = await CanEditSessionAsync(sessionId, userId, ct);
            if (!resultIsEdit.IsSuccess && !isBackground.HasValue)
                return Result.Failure<Unit, ErrorResponse>(resultIsEdit.Failure);

            bool isEdit = resultIsEdit.Success;
            if (!isEdit && !isBackground.HasValue)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "You do not have sufficient permissions to edit this session."
                });

            var session = await GetSessionAsync(new SessionWithPlayersSpec(sessionId), ct);
            if (session == null)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse { Error = "Session not found." });

            if(session.PlayerSessions == null || session.PlayerSessions.Count == 0)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse { Error = "Players Session not found." });

            if (session.PlayerSessions.Any(ps => !ps.IsCompleted))
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                { Error = "Not all players have completed the task yet." });

            var resultAssign = await AssignBestResult(session, ct, commit: false);
            if (!resultAssign.IsSuccess)
                return resultAssign;

            var list = session.PlayerSessions.ToList();
            foreach (var playerSession in list)
            {
                if (playerSession?.Player != null)
                {
                    await _gameEventDispatcher.DispatchAsync(
                        new GameEventContext
                        {
                            EventType = GameEventType.MatchPlayed,
                            PlayerId = playerSession.Player.Id
                        },
                        ct,
                        false
                    );
                }
            }

            var resultAdd = await AddPlayerCountGames(list, ct, commit: false);
            if (!resultAdd.IsSuccess)
                return resultAdd;

            var resultStart = await FinishGameInDbAsync(sessionId, ct);
            if (!resultStart.IsSuccess)
                return resultStart;

            return Result.Success<Unit, ErrorResponse>(Unit.Value);
        }
        private async Task<Result<Unit, ErrorResponse>> AddPlayerCountGames
            (List<PlayerSession> playersSessions, CancellationToken ct, bool commit = true)
        {
            try
            {
                foreach (var playerSession in playersSessions)
                {
                    await _unitOfWork.PlayerRepository.AddCountGamePlayerAsync(playerSession.IdPlayer, ct);
                }

                if (commit)
                    await _unitOfWork.CommitAsync(ct);
                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating Player");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when update player."
                });
            }
        }
        private async Task<Result<Unit, ErrorResponse>> AssignBestResult(Session session, CancellationToken ct, bool commit = true)
        {
            var best = session.PlayerSessions
                .Where(p => p.IsCompleted && p.FinishTask.HasValue && p.Memory.HasValue)
                .Select(p => {
                    // парсинг в секунды
                    var parsedTime = double.TryParse(p.Time, NumberStyles.Any, CultureInfo.InvariantCulture, out var t)
                        ? t : double.MaxValue;

                    // вычисление потраченного времени (в секундах)
                    var timeSpentSeconds = (p.FinishTask.Value - session.DateStartGame)?.TotalSeconds ?? double.MaxValue;

                    return new
                    {
                        Player = p,
                        ParsedTime = parsedTime,
                        TimeSpent = timeSpentSeconds
                    };
                })
                .OrderBy(p => p.ParsedTime)
                .ThenBy(p => p.TimeSpent)
                .ThenBy(p => p.Player.Memory.Value)
                .Select(p => p.Player)
                .FirstOrDefault();

            if (best == null)
                return Result.Success<Unit, ErrorResponse>(Unit.Value);

            session.WinnerId = best.IdPlayer;
            var resultUpdate = await UpdateSessionInDbAsync(session, ct, commit: false);
            if (!resultUpdate.IsSuccess)
                return Result.Failure<Unit, ErrorResponse>(resultUpdate.Failure);

            if(session.PlayerSessions.Count > 1)
            {
                foreach (var playerSession in session.PlayerSessions)
                {
                    if (playerSession?.Player != null && best.IdPlayer != playerSession.Player.Id)
                    {
                        await _gameEventDispatcher.DispatchAsync(
                            new GameEventContext
                            {
                                EventType = GameEventType.Defeat,
                                PlayerId = playerSession.Player.Id
                            },
                            ct,
                            false
                        );
                    }
                }

                var resultAdd = await _playerService.AddVictoryPlayerInDbAsync(best.IdPlayer, ct, commit: false);
                if (!resultAdd.IsSuccess)
                    return Result.Failure<Unit, ErrorResponse>(resultAdd.Failure);

                await _gameEventDispatcher.DispatchAsync(
                    new GameEventContext
                    {
                        EventType = GameEventType.Victory,
                        PlayerId = best.IdPlayer
                    },
                    ct,
                    commit
                );
            }

            return Result.Success<Unit, ErrorResponse>(Unit.Value);
        }
        public async Task<Result<bool, ErrorResponse>> CanAccessSessionPlayersAsync
            (int sessionId, string userId, CancellationToken ct)
        {
            var session = await GetSessionAsync(new SessionWithPlayersSpec(sessionId), ct);
            if (session == null)
                return Result.Failure<bool, ErrorResponse>(new ErrorResponse { Error = "Session not found." });

            bool isPrivate = session.State == SessionState.Private;

            var checkResult = ValidationHelper.CheckUserId<bool>(userId);
            if (!checkResult.IsSuccess)
                return Result.Success<bool, ErrorResponse>(!isPrivate);

            bool isParticipant = session.PlayerSessions.Any(p => p.IdPlayer == userId);
            var roles = await _playerService.GetRolesAsync(userId);
            bool isEdit = BusinessRules.IsModerationRole(roles);

            if (isPrivate && !isParticipant && !isEdit)
                return Result.Success<bool, ErrorResponse>(false);

            return Result.Success<bool, ErrorResponse>(true);
        }
        public async Task<Result<bool, ErrorResponse>> CanEditSessionAsync
            (int sessionId, string userId, CancellationToken ct)
        {
            var checkResult = ValidationHelper.CheckUserId<bool>(userId);
            if (!checkResult.IsSuccess)
                return Result.Success<bool, ErrorResponse>(false);

            var session = await GetSessionAsync(new SessionByIdSpec(sessionId), ct);
            if (session == null)
                return Result.Failure<bool, ErrorResponse>(new ErrorResponse 
                { 
                    Error = "Session not found." 
                });

            var roles = await _playerService.GetRolesAsync(userId);
            bool isEditSession = BusinessRules.IsEditSession(userId, session, roles);

            return Result.Success<bool, ErrorResponse>(isEditSession);
        }
        public async Task<Result<Session, ErrorResponse>> CreateSessionAsync
            (string userId, SessionDto dto, CancellationToken ct)
        {
            var spec = Specification<PlayerSession>.Combine(
                new PlayerSessionDefaultIncludesSpec(),
                new PlayerSessionByIdSpec(idPlayer: userId)
            );
            var sessions = await _unitOfWork.PlayerSessionRepository.GetListPlayerSessionByIdAsync(spec, ct);
            bool isActive = sessions.Any(s => s.IsCompleted == false);
            if (isActive)
                return Result.Failure<Session, ErrorResponse>(new ErrorResponse
                {
                    Error = "You already have an active session."
                });

            dto.CreatorId = userId;
            dto.DateCreating = DateTime.Now;

            Session session = new Session();
            _mapper.Map(dto, session);

            var addResult = await AddSessionInDbAsync(session, ct);
            if (!addResult.IsSuccess)
                return Result.Failure<Session, ErrorResponse>(addResult.Failure);

            return Result.Success<Session, ErrorResponse>(session);
        }
        public async Task<Result<Unit, ErrorResponse>> UpdateSessionAsync
            (string userId, SessionDto dto, CancellationToken ct)
        {
            var resultIsEdit = await CanEditSessionAsync(dto.IdSession.Value, userId, ct);
            if (!resultIsEdit.IsSuccess)
                return Result.Failure<Unit, ErrorResponse>(resultIsEdit.Failure);

            bool isEdit = resultIsEdit.Success;
            if(!isEdit)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse 
                { 
                    Error = "You do not have sufficient permissions to edit this session."
                });

            var session = await GetSessionAsync(new SessionByIdSpec(dto.IdSession.Value), ct); // получаем из БД
            if (session == null)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse { Error = "Session not found." });

            if (BusinessRules.IsStartetSession(session))
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "The game has started, session data cannot be changed."
                });

            _mapper.Map(dto, session);

            if(session.TaskId != null)
            {
                var task = await _unitOfWork.TaskRepository.GetTaskProgrammingAsync(session.TaskId.Value, ct);
                if (task == null)
                    return Result.Failure<Unit, ErrorResponse>(new ErrorResponse { Error = "Task not found." });

                if (session.LangProgrammingId != task.LangProgrammingId || dto.TaskId != session.TaskId)
                {
                    var resultDeleting = await DeletingTaskToSessionInDbAsync(session.IdSession, ct, commit: false);
                    if (!resultDeleting.IsSuccess)
                        return Result.Failure<Unit, ErrorResponse>(resultDeleting.Failure);
                }
            }

            var resultUpdate = await UpdateSessionInDbAsync(session, ct);
            if(!resultUpdate.IsSuccess)
                return Result.Failure<Unit, ErrorResponse>(resultUpdate.Failure);

            return Result.Success<Unit, ErrorResponse>(Unit.Value);
        }
        public async Task<Result<SessionDto, ErrorResponse>> SelectTaskForSessionAsync
            (string userId, int idSession, int idTask, CancellationToken ct)
        {
            var session = await GetSessionAsync(new SessionByIdSpec(idSession), ct);
            if (session == null)
                return Result.Failure<SessionDto, ErrorResponse>(new ErrorResponse{ Error = "Session not found." });

            var task = await _unitOfWork.TaskRepository.GetTaskProgrammingAsync(idTask, ct);
            if (task == null)
                return Result.Failure<SessionDto, ErrorResponse>(new ErrorResponse { Error = "Task not found." });

            if(BusinessRules.IsStartetSession(session))
                return Result.Failure<SessionDto, ErrorResponse>(new ErrorResponse
                {
                    Error = "The game has been started, the task cannot be changed."
                });

            if (session.LangProgrammingId != task.LangProgrammingId)
                return Result.Failure<SessionDto, ErrorResponse>(new ErrorResponse 
                { 
                    Error = "The programming language of the selected task does not match the session." 
                });

            var dto = _mapper.Map<SessionDto>(session);
            dto.TaskId = idTask;

            var resultUpdate = await UpdateSessionAsync(userId, dto, ct);
            if (!resultUpdate.IsSuccess)
                return Result.Failure<SessionDto, ErrorResponse>(resultUpdate.Failure);

            return Result.Success<SessionDto, ErrorResponse>(dto);
        }
        public async Task<Result<Unit, ErrorResponse>> DeletingSessionAsync
            (string userId, int idSession, CancellationToken ct)
        {
            var session = await GetSessionAsync(new SessionByIdSpec(idSession), ct);
            if(session == null)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse { Error = "Session not found."});

            var dto = _mapper.Map<SessionDto>(session);

            var resultIsEdit = await CanEditSessionAsync(dto.IdSession.Value, userId, ct);
            if (!resultIsEdit.IsSuccess)
                return Result.Failure<Unit, ErrorResponse>(resultIsEdit.Failure);

            bool isEdit = resultIsEdit.Success;
            if (!isEdit)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "You do not have sufficient permissions to edit this session."
                });

            if (BusinessRules.IsFinishSession(session) && !BusinessRules.IsEditRole(await _playerService.GetRolesAsync(userId)))
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "You can't delete a closed session."
                });

            var resultDeleting = await DelSessionInDbAsync(idSession, ct);
            if (!resultDeleting.IsSuccess)
                return Result.Failure<Unit, ErrorResponse>(resultDeleting.Failure);

            return Result.Success<Unit, ErrorResponse>(Unit.Value);
        }
        public async Task<Result<bool, ErrorResponse>> CheckPasswordAsync
            (string password, int idSession, CancellationToken ct)
        {
            var session = await GetSessionAsync (new SessionByIdSpec(idSession), ct);
            if (session == null)
                return Result.Failure<bool, ErrorResponse>(new ErrorResponse { Error = "Session not found" });

            return Result.Success<bool, ErrorResponse>(
                session.State == SessionState.Public || session.Password == password
            );
        }
        public async Task<Result<int, ErrorResponse>> GetCountCompletedTaskAsync(int idSession, CancellationToken ct)
        {
            var spec = Specification<PlayerSession>.Combine(
                new PlayerSessionDefaultIncludesSpec(),
                new PlayerSessionByIdSpec(idSession)
            );
            var playerSessionList = await _unitOfWork.PlayerSessionRepository.GetListPlayerSessionByIdAsync(spec, ct);

            if (playerSessionList == null)
                return Result.Failure<int, ErrorResponse>(new ErrorResponse { Error = "Session not found" });

            var completedCount = playerSessionList.Count(p => p.IsCompleted);

            return Result.Success<int, ErrorResponse>(completedCount);
        }
        public async Task<Session> GetSessionAsync(ISpecification<Session> spec, CancellationToken ct)
        {
            return await _unitOfWork.SessionRepository.GetSessionAsync(spec, ct);
        }

        //------------ DATABASE ------------
        private async Task<Result<Unit, ErrorResponse>> StartGameInDbAsync(int idSession, CancellationToken ct, bool commit = true)
        {
            try
            {
                await _unitOfWork.SessionRepository.StartGameAsync(idSession, ct);
                if (commit)
                    await _unitOfWork.CommitAsync(ct);
                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error start game Session");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when start game session."
                });
            }
        }
        private async Task<Result<Unit, ErrorResponse>> FinishGameInDbAsync(int idSession, CancellationToken ct, bool commit = true)
        {
            try
            {
                await _unitOfWork.SessionRepository.FinishGameAsync(idSession, ct);
                if (commit)
                    await _unitOfWork.CommitAsync(ct);
                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finish game Session");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when finish game session."
                });
            }
        }
        public async Task<PlayerSession> GetVinnerAsync(int idSession, CancellationToken ct)
        {
            return await _unitOfWork.SessionRepository.GetVinnerAsync(idSession, ct);
        }
        public async Task<Result<Unit, ErrorResponse>> AddSessionInDbAsync(Session session, CancellationToken ct, bool commit = true)
        {
            try
            {
                await _unitOfWork.SessionRepository.AddSessionAsync(session, ct);
                if (commit)
                    await _unitOfWork.CommitAsync(ct);
                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding Session");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse 
                { 
                    Error = "Database error when adding session." 
                });
            }
        }
        private async Task<Result<Unit, ErrorResponse>> AddTaskToSessionInDbAsync(int idSession, int idTask, CancellationToken ct, bool commit = true)
        {
            try
            {
                await _unitOfWork.SessionRepository.AddTaskToSession(idSession, idTask, ct);
                if (commit)
                    await _unitOfWork.CommitAsync(ct);
                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding Task to Session");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when Task to Session."
                });
            }
        }
        private async Task<Result<Unit, ErrorResponse>> DeletingTaskToSessionInDbAsync(int idSession, CancellationToken ct, bool commit = true)
        {
            try
            {
                await _unitOfWork.SessionRepository.DelTaskToSession(idSession, ct);
                if (commit)
                    await _unitOfWork.CommitAsync(ct);
                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting task to Session");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when deleting task to Session."
                });
            }
        }
        public async Task<Session> GetSessionInDbAsync(int id, CancellationToken ct)
        {
            return await _unitOfWork.SessionRepository.GetSessionAsync(new SessionWithPlayersSpec(id), ct);
        }
        private async Task ChangePasswordSessionInDbAsync(int idSession, string password, CancellationToken ct, bool commit = true)
        {
            await _unitOfWork.SessionRepository.ChangePasswordSessionAsync(idSession, password, ct);
            if (commit)
                await _unitOfWork.CommitAsync(ct);
        }
        private async Task<Result<Unit, ErrorResponse>> DelSessionInDbAsync(int id, CancellationToken ct, bool commit = true)
        {
            try
            {
                await _unitOfWork.SessionRepository.DelSessionAsync(id, ct);
                if (commit)
                    await _unitOfWork.CommitAsync(ct);
                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Session");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Database error when deleting session."
                });
            }
        }
        public async Task<List<Player>> GetListPlayerFromSessionAsync(int idSession, CancellationToken ct)
        {
            return await _unitOfWork.SessionRepository.GetListPlayerFromSessionAsync(idSession, ct);
        }
        public async Task<int> GetPlayerCountInSessionAsync(int idSession, CancellationToken ct)
        {
            return await _unitOfWork.SessionRepository.GetPlayerCountInSessionAsync(idSession, ct);
        }
        public async Task<List<Session>> GetListSessionAsync(IFilter<Session>? filter, CancellationToken ct)
        {
            return await _unitOfWork.SessionRepository.GetListSessionAsync(new SessionsByFilter(filter), ct);
        }
        public async Task DeleteExpiredSessionsInDbAsync(DateTime dateTime, CancellationToken ct, bool commit = true)
        {
            try
            {
                var idSessionsDelete = await _unitOfWork.SessionRepository.DeleteExpiredSessionsAsync(dateTime, ct);
                if (commit)
                    await _unitOfWork.CommitAsync(ct);
                foreach (var id in idSessionsDelete)
                {
                    await _sessionNotificationService.NotifySessionDeletedAllAsync(id);
                    await _sessionNotificationService.NotifySessionDeletedGroupAsync(id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when executing a method (DeleteExpiredSessionsAsync)");
            }
        }
        public async Task FinishExpiredSessionsInDbAsync(DateTime dateTime, CancellationToken ct, bool commit = true)
        {
            {
                try
                {
                    var idSessionsFinishid = await _unitOfWork.SessionRepository.FinishExpiredSessionsAsync(dateTime, ct);
                    if (commit)
                        await _unitOfWork.CommitAsync(ct);

                    foreach (var id in idSessionsFinishid)
                    {
                        await _sessionNotificationService.NotifyFinishGameAsync(id);
                        await FinishGameAsync(id, null, ct, isBackground: true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error when executing a method (FinishExpiredSessionsAsync)");
                }
            }
        }
        private async Task<Result<Unit, ErrorResponse>> UpdateSessionInDbAsync(Session session, CancellationToken ct, bool commit = true)
        {
            try
            {
                await _unitOfWork.SessionRepository.UpdateSession(session);
                if (commit)
                    await _unitOfWork.CommitAsync(ct);
                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating Session");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse 
                { 
                    Error = "Database error when update session." 
                });
            }
        }
    }
}
