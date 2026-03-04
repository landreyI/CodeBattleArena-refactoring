
using CodeBattleArena.Application.Features.Sessions.Models;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.PlayerSessions;
using CodeBattleArena.Domain.ProgrammingLanguages;
using CodeBattleArena.Domain.ProgrammingTasks;
using CodeBattleArena.Domain.Sessions.Events.Integration;
using CodeBattleArena.Domain.Sessions.Events.Internal;
using CodeBattleArena.Domain.Sessions.Value_Objects;
using System.Globalization;

namespace CodeBattleArena.Domain.Sessions
{
    public class Session : BaseEntity<Guid>
    {
        public string Name { get; set; }

        // Настройки доступа и лимиты
        public SessionAccess Access { get; private set; }
        public int? TimePlay { get; private set; } // in minutes

        // Жизненный цикл
        public GameStatus Status { get; private set; } = GameStatus.Waiting;

        public Guid ProgrammingLangId { get; private set; }
        public virtual ProgrammingLang? ProgrammingLang { get; private set; }
        public Guid? ProgrammingTaskId { get; private set; }
        public virtual ProgrammingTask? ProgrammingTask { get; private set; }

        public Guid CreatorId { get; private set; }
        public Guid? WinnerId { get; private set; }

        public DateTime DateCreating { get; private set; }
        public DateTime? DateStartGame { get; private set; }


        private readonly List<PlayerSession> _playerSessions = new();
        public virtual ICollection<PlayerSession>? PlayerSessions => _playerSessions.AsReadOnly();

        private Session() { } // Для EF

        private Session(string name, Guid creatorId, SessionAccess access, Guid langId, int? timePlay = default)
        {
            Name = name;
            CreatorId = creatorId;
            Access = access;
            ProgrammingLangId = langId;
            DateCreating = DateTime.UtcNow;
            TimePlay = timePlay;

            AddDomainEvent(new SessionCreatedIntegrationEvent(this));
        }

        public static Result<Session> Create(
            string name,
            Guid creatorId,
            SessionState type,
            int maxPeople,
            string? password,
            Guid langId,
            int? timePlay = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = "room_" + Guid.NewGuid().ToString().Substring(0, 8);
            else if (name.Length > 20)
                return Result<Session>.Failure(new Error("session.name", "Name cannot be longer than 20 characters."));

            if (creatorId == Guid.Empty)
                return Result<Session>.Failure(new Error("session.creatorId", "CreatorId cannot be empty."));

            if (langId == Guid.Empty)
                return Result<Session>.Failure(new Error("session.langId", "LangId cannot be empty."));


            if (maxPeople <= 0 || maxPeople > 10)
                return Result<Session>.Failure(new Error("session.max_people", "Participants must be between 1 and 10."));

            if (type == SessionState.Private && string.IsNullOrWhiteSpace(password))
                return Result<Session>.Failure(new Error("session.password", "Password is required for private sessions."));

            if (timePlay.HasValue && (timePlay.Value < 5 || timePlay.Value > 180))
                return Result<Session>.Failure(new Error("session.time_play", "TimePlay must be between 5 and 180 minutes."));


            var access = new SessionAccess(type, maxPeople, password);

            return Result<Session>.Success(new Session(name, creatorId, access, langId, timePlay));
        }

        public Result Update(
            string? name = null,
            Guid? programmingLangId = null,
            SessionState? type = null,
            int? maxPeople = null,
            string? password = null,
            int? timePlay = null,
            Guid? taskId = null)
        {
            if (Status != GameStatus.Waiting)
                return Result.Failure(new Error("session.invalid_status", "Can only update settings for a waiting session."));

            if (name != null)
            {
                if (string.IsNullOrWhiteSpace(name))
                    name = "room_" + Guid.NewGuid().ToString().Substring(0, 8);
                else if (name.Length > 20)
                    return Result.Failure(new Error("session.name", "Name cannot be longer than 20 characters."));

                Name = name;
            }

            if (programmingLangId.HasValue)
            {
                if (programmingLangId.Value == Guid.Empty)
                    return Result.Failure(new Error("session.langId", "LangId cannot be empty."));

                ProgrammingLangId = programmingLangId.Value;
            }

            if (timePlay.HasValue)
            {
                if (timePlay.Value < 5 || timePlay.Value > 180)
                    return Result.Failure(new Error("session.time_play", "TimePlay must be between 5 and 180 minutes."));

                TimePlay = timePlay.Value;
            }

            if (taskId.HasValue)
            {
                ProgrammingTaskId = taskId.Value;
            }

            if (type.HasValue || maxPeople.HasValue || password != null)
            {
                var newType = type ?? Access.Type;
                var newMaxPeople = maxPeople ?? Access.MaxPeople;
                var newPassword = password ?? Access.Password;

                if (newMaxPeople <= 0 || newMaxPeople > 10)
                    return Result.Failure(new Error("session.max_people", "Participants must be between 1 and 10."));

                if (newType == SessionState.Private && string.IsNullOrWhiteSpace(newPassword))
                    return Result.Failure(new Error("session.password", "Password is required for private sessions."));

                Access = new SessionAccess(newType, newMaxPeople, newPassword);
            }

            AddDomainEvent(new SessionUpdatedIntegrationEvent(this));
            return Result.Success();
        }

        public void Delete()
        {
            AddDomainEvent(new SessionDeletedIntegrationEvent(this.Id));
        }
        public Result Start()
        {
            if (Status != GameStatus.Waiting)
                return Result.Failure(new Error("session.invalid_status", "Can only start a waiting session."));

            Status = GameStatus.InProgress;
            DateStartGame = DateTime.UtcNow;

            AddDomainEvent(new SessionStartedIntegrationEvent(this));

            return Result.Success();
        }

        public Result Finish()
        {
            if (Status != GameStatus.InProgress) return Result.Failure(new Error("session.game_status", "You can't finish a game that hasn't started yet."));

            if (_playerSessions.Any(ps => !ps.IsCompleted))
                return Result.Failure(new Error("session.not_all_finished", "Not all players have completed the task yet."));

            var best = _playerSessions
                .Where(p => p.IsCompleted && p.Result.FinishTask.HasValue && p.Result.Memory.HasValue)
                .Select(p =>
                {
                    double parsedTime = double.TryParse(p.Result.Time, NumberStyles.Any, CultureInfo.InvariantCulture, out var t) ? t : double.MaxValue;
                    double timeSpent = (p.Result.FinishTask.Value - (DateStartGame ?? DateTime.UtcNow)).TotalSeconds;
                    return new { Player = p, ParsedTime = parsedTime, TimeSpent = timeSpent };
                })
                .OrderBy(p => p.ParsedTime)
                .ThenBy(p => p.TimeSpent)
                .ThenBy(p => p.Player.Result.Memory)
                .Select(p => p.Player)
                .FirstOrDefault();

            Status = GameStatus.Finished;
            WinnerId = best?.PlayerId;

            AddDomainEvent(new GameFinishedInternalEvent(this));
            AddDomainEvent(new SessionFinishedIntegrationEvent(this));

            return Result.Success();
        }

        /// <remarks>
        /// <para><b>IMPORTANT:</b> The 'PlayerSessions' collection MUST be eagerly loaded (e.g., using .Include()) 
        /// before calling this method.</para>
        /// </remarks>
        public Result CanJoin(string? password = default)
        {
            if (Status != GameStatus.Waiting)
                return Result.Failure(new Error("session.game_status", "The game has already started, new participants cannot join."));

            if (_playerSessions.Count >= Access.MaxPeople)
                return Result.Failure(new Error("session.max_people", "The maximum number of players has been reached."));

            if (Access.Type == SessionState.Private && Access.Password != password)
                return Result.Failure(new Error("session.password", "You may have entered the wrong password."));

            return Result.Success();
        }

        /// <remarks>
        /// <para><b>IMPORTANT:</b> The 'PlayerSessions' collection MUST be eagerly loaded (e.g., using .Include()) 
        /// before calling this method.</para>
        /// </remarks>
        public Result AddPlayer(PlayerSession playerSession, string? password = default)
        {
            var canJoinResult = CanJoin(password);
            if (canJoinResult.IsFailure) return canJoinResult;

            if (_playerSessions.Any(ps => ps.PlayerId == playerSession.PlayerId))
                return Result.Failure(new Error("session.player_exists", "Player is already in the session."));

            _playerSessions.Add(playerSession);

            AddDomainEvent(new SessionJoinedIntegrationEvent(this.Id, playerSession.PlayerId));
            return Result.Success();
        }

        /// <remarks>
        /// <para><b>IMPORTANT:</b> The 'PlayerSessions' collection MUST be eagerly loaded (e.g., using .Include()) 
        /// before calling this method.</para>
        /// </remarks>
        public Result RemovePlayer(Guid playerId)
        {
            var playerSession = _playerSessions.FirstOrDefault(ps => ps.PlayerId == playerId);
            if (playerSession == null)
                return Result.Failure(new Error("session.player_not_found", "Player is not in the session.", 404));

            _playerSessions.Remove(playerSession);

            // Передача прав создателя
            if (playerId == this.CreatorId)
            {
                var nextOwner = _playerSessions.FirstOrDefault();
                if (nextOwner != null)
                {
                    this.CreatorId = nextOwner.PlayerId;
                }
            }

            // Логика завершения (только если игра шла)
            if (Status == GameStatus.InProgress && _playerSessions.Count == 1)
            {
                Status = GameStatus.Finished;
                WinnerId = _playerSessions.First().PlayerId;

                AddDomainEvent(new GameFinishedInternalEvent(this));
                AddDomainEvent(new SessionFinishedIntegrationEvent(this));
            }

            // Если комната опустела
            if (!_playerSessions.Any())
                AddDomainEvent(new SessionDeletedIntegrationEvent(this.Id));
            else
                AddDomainEvent(new SessionUnjoinedIntegrationEvent(this.Id, playerId));

            return Result.Success();
        }

        /// <remarks>
        /// <para><b>IMPORTANT:</b> The 'PlayerSessions' collection MUST be eagerly loaded (e.g., using .Include()) 
        /// before calling this method.</para>
        /// </remarks>
        public Result CheckViewAccess(Guid? viewerId, Guid targetPlayerId)
        {
            // Если сессия приватная и зритель не участник — нельзя
            bool viewerIsParticipant = viewerId.HasValue && _playerSessions.Any(ps => ps.PlayerId == viewerId.Value);
            if (Access.Type == SessionState.Private && !viewerIsParticipant)
                return Result.Failure(new Error("session.access_denied", "This is a private session."));

            // Анти-чит: если смотрим чужой код
            if (viewerId != targetPlayerId)
            {
                var viewerSession = PlayerSessions.FirstOrDefault(ps => ps.PlayerId == viewerId);
                // Если зритель — участник, и он еще не закончил — запрет
                if (viewerSession != null && !viewerSession.IsCompleted)
                    return Result.Failure(new Error("session.access_denied", "Finish your task before viewing others."));
            }

            return Result.Success();
        }

        /// <remarks>
        /// <para><b>IMPORTANT:</b> The 'PlayerSessions' collection MUST be eagerly loaded (e.g., using .Include()) 
        /// before calling this method.</para>
        /// </remarks>
        public Result<CompletionCount> GetCompletionCount()
        {
            return Result<CompletionCount>.Success(new CompletionCount
            (
                _playerSessions.Count(ps => ps.IsCompleted),
                _playerSessions.Count
            ));
        }

        /// <remarks>
        /// <para><b>IMPORTANT:</b> The 'PlayerSessions' collection MUST be eagerly loaded (e.g., using .Include()) 
        /// before calling this method.</para>
        /// </remarks>
        public Result FinishTaskPlayer(Guid playerId)
        {
            if (Status != GameStatus.InProgress)
                return Result.Failure(new Error("session.status", "You can only finish tasks while the game is in progress."));

            var playerSession = PlayerSessions!.FirstOrDefault(ps => ps.PlayerId == playerId);
            if (playerSession is null)
                return Result.Failure(new Error("session.player", "Player not found in session", 404));

            var resultUpdate = playerSession.FinishTask();
            if (resultUpdate.IsFailure)
                return Result.Failure(resultUpdate.Error);

            return Result.Success();
        }
    }
}
