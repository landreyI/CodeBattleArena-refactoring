using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.Players;

namespace CodeBattleArena.Domain.Friendships
{
    public class Friendship : BaseEntity<Guid>
    {
        public Guid SenderId { get; private set; }
        public virtual Player? Sender { get; private set; }

        public Guid ReceiverId { get; private set; }
        public virtual Player? Receiver { get; private set; }

        public FriendshipStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ConfirmedAt { get; private set; }

        private Friendship() { } // Для EF

        private Friendship(Guid senderId, Guid receiverId)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
            Status = FriendshipStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public static Result<Friendship> Create(Guid senderId, Guid receiverId)
        {
            if (senderId == Guid.Empty || receiverId == Guid.Empty)
                return Result<Friendship>.Failure(new Error("friendship.invalid_id", "Player IDs cannot be empty."));

            if (senderId == receiverId)
                return Result<Friendship>.Failure(new Error("friendship.self_addition", "You cannot add yourself as a friend."));

            return Result<Friendship>.Success(new Friendship(senderId, receiverId));
        }

        public Result Accept()
        {
            if (Status == FriendshipStatus.Accepted)
                return Result.Failure(new Error("friendship.already_accepted", "You are already friends."));

            if (Status == FriendshipStatus.Blocked)
                return Result.Failure(new Error("friendship.blocked", "Cannot accept a request from a blocked user."));

            Status = FriendshipStatus.Accepted;
            ConfirmedAt = DateTime.UtcNow;
            return Result.Success();
        }

        public Result Block()
        {
            if (Status == FriendshipStatus.Blocked)
                return Result.Failure(new Error("friendship.already_blocked", "User is already blocked."));

            Status = FriendshipStatus.Blocked;
            ConfirmedAt = DateTime.UtcNow;
            return Result.Success();
        }
    }
}