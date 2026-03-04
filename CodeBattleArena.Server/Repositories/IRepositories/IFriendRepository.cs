using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Untils;

namespace CodeBattleArena.Server.IRepositories
{
    public interface IFriendRepository
    {
        Task<Result<Unit, ErrorResponse>> AddFriendAsync(string requesterId, string addresseeId, CancellationToken cancellationToken);
        Task ApproveFriendshipAsync(int idFriend, CancellationToken cancellationToken);
        Task DeleteFriendAsync(string playerId1, string playerId2, CancellationToken cancellationToken);
        Task DeleteFriendAsync(int idFriend, CancellationToken cancellationToken);
        Task<List<Friend>> GetAllFriendsAsync(string playerId, CancellationToken cancellationToken);
        Task<List<Friend>> GetFriendshipFriends(string playerId, CancellationToken cancellationToken);
        Task<Friend> GetFriendAsync(string playerId1, string playerId2, CancellationToken cancellationToken);
        Task<Friend> GetFriendAsync(int idFriend, CancellationToken cancellationToken);
    }
}
