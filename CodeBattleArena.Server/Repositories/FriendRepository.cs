using CodeBattleArena.Server.Data;
using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Untils;
using Microsoft.EntityFrameworkCore;

namespace CodeBattleArena.Server.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private readonly AppDBContext _context;

        public FriendRepository(AppDBContext context)
        {
            _context = context;
        }
        public async Task<Result<Unit, ErrorResponse>> AddFriendAsync(string requesterId, string addresseeId, CancellationToken cancellationToken)
        {
            if (requesterId == addresseeId)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "You can't add yourself as a friend"
                });

            bool exists = await _context.Friends
                .AnyAsync(f =>
                    (f.RequesterId == requesterId && f.AddresseeId == addresseeId) ||
                    (f.RequesterId == addresseeId && f.AddresseeId == requesterId), cancellationToken);

            if (exists)
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "You have already been sent a friend invitation, or you are already friends\r\n"
                });

            var friend = new Friend
            {
                RequesterId = requesterId,
                AddresseeId = addresseeId,
                IsFriendship = false,
                FriendshipDate = DateTime.UtcNow
            };

            await _context.Friends.AddAsync(friend, cancellationToken);
            return Result.Success<Unit, ErrorResponse>(Unit.Value);
        }


        public async Task ApproveFriendshipAsync(int idFriend, CancellationToken cancellationToken)
        {
            var friend = await _context.Friends.FindAsync(idFriend, cancellationToken);
            if(friend != null)
            {
                friend.IsFriendship = true;
                friend.FriendshipDate = DateTime.UtcNow;
                _context.Friends.Update(friend);
            }
        }

        public async Task DeleteFriendAsync(int idFriend, CancellationToken cancellationToken)
        {
            var friend = await _context.Friends.FindAsync(idFriend, cancellationToken);
            if(friend != null)
                _context.Friends.Remove(friend);
        }
        public async Task DeleteFriendAsync(string playerId1, string playerId2, CancellationToken cancellationToken)
        {
            if (playerId1 == playerId2)
                return;

            var friend = await _context.Friends
                .FirstOrDefaultAsync(f =>
                    (f.RequesterId == playerId1 && f.AddresseeId == playerId2) ||
                    (f.RequesterId == playerId2 && f.AddresseeId == playerId1),
                    cancellationToken);

            if (friend == null)
                return;

            _context.Friends.Remove(friend);
        }
        public async Task<List<Friend>> GetAllFriendsAsync(string playerId, CancellationToken cancellationToken)
        {
            return await _context.Friends
                 .Include(f => f.Addressee)
                 .Include(f => f.Requester)
                 .Where(f => f.RequesterId == playerId || f.AddresseeId == playerId)
                 .ToListAsync(cancellationToken);
        }
        public async Task<List<Friend>> GetFriendshipFriends(string playerId, CancellationToken cancellationToken)
        {
            return await _context.Friends
                 .Include(f => f.Addressee)
                 .Include(f => f.Requester)
                 .Where(f => f.IsFriendship && (f.RequesterId == playerId || f.AddresseeId == playerId))
                 .ToListAsync(cancellationToken);
        }
        public async Task<Friend?> GetFriendAsync(string playerId1, string playerId2, CancellationToken cancellationToken)
        {
            return await _context.Friends
                .FirstOrDefaultAsync(
                    f => f.IsFriendship &&
                        ((f.RequesterId == playerId1 && f.AddresseeId == playerId2) ||
                         (f.RequesterId == playerId2 && f.AddresseeId == playerId1)),
                    cancellationToken);
        }

        public async Task<Friend> GetFriendAsync(int idFriend, CancellationToken cancellationToken)
        {
            return await _context.Friends
                 .FirstOrDefaultAsync(f => f.IdFriend == idFriend, cancellationToken);
        }
    }
}
