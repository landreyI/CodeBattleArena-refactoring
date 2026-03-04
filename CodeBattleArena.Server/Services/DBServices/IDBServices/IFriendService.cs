using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Untils;

namespace CodeBattleArena.Server.Services.DBServices.IDBServices
{
    public interface IFriendService
    {
        Task<List<Friend>> GetAllFriendsAsync(string playerId, CancellationToken cancellationToken);
        Task<List<Friend>> GetFriendshipFriends(string playerId, CancellationToken cancellationToken);
        Task<Friend> GetFriendAsync(string playerId1, string playerId2, CancellationToken cancellationToken);
        Task<Friend> GetFriendAsync(int idFriend, CancellationToken cancellationToken);
        Task<Result<Unit, ErrorResponse>> AddFriendAsync(string requesterId, string addresseeId, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> ApproveFriendshipAsync(string authUserId, int idFriend, CancellationToken cancellationToken, bool commit = true);
        Task<Result<Unit, ErrorResponse>> DeleteFriendAsync(string authUserId, int idFriend, CancellationToken cancellationToken, bool commit = true);
    }
}
