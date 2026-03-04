using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Players;
using CodeBattleArena.Domain.PlayerSessions.Events.Integration;
using CodeBattleArena.Domain.PlayerSessions.Value_Objects;
using CodeBattleArena.Domain.Sessions;

namespace CodeBattleArena.Domain.PlayerSessions
{
    public class PlayerSession : BaseEntity<Guid>
    {
        public Guid PlayerId { get; private set; }
        public virtual Player? Player { get; private set; }

        public Guid SessionId { get; private set; }
        public virtual Session? Session { get; private set; }

        public SubmissionResult? Result { get; private set; }
        public bool IsCompleted { get; private set; }

        private PlayerSession() { } // Для EF

        private PlayerSession(Guid playerId, Guid sessionId)
        {
            PlayerId = playerId;
            SessionId = sessionId;
            IsCompleted = false;
        }

        public static Result<PlayerSession> Create(Guid playerId, Guid sessionId)
        {
            if (playerId == Guid.Empty)
                return Result<PlayerSession>.Failure(new Error("player_session.invalid_player", "Player ID cannot be empty."));

            if (sessionId == Guid.Empty)
                return Result<PlayerSession>.Failure(new Error("player_session.invalid_session", "Session ID cannot be empty."));

            return Result<PlayerSession>.Success(new PlayerSession(playerId, sessionId));
        }

        public Result SubmitSolution(string code, string? time, int? memory)
        {
            if (IsCompleted)
                return Common.Result.Failure(new Error("player_session.already_completed", "The solution for this session has already been submitted."));

            if (string.IsNullOrWhiteSpace(code))
                return Common.Result.Failure(new Error("player_session.empty_code", "Submitted code cannot be empty."));

            Result = new SubmissionResult(code, time, memory);

            AddDomainEvent(new UpdatePlayerSessionIntegrationEvent(this));

            return Common.Result.Success();
        }

        public Result FinishTask()
        {
            if (IsCompleted)
                return Common.Result.Failure(new Error("player_session.already_completed", "The task for this session has already been completed."));
            IsCompleted = true;

            AddDomainEvent(new PlayerFinishedTaskIntegrationEvent(this.Id, this.PlayerId));

            return Common.Result.Success();
        }
    }
}