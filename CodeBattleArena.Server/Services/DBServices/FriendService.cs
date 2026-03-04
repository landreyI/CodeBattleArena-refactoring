using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Untils;
using Microsoft.EntityFrameworkCore;
using static Azure.Core.HttpHeader;

namespace CodeBattleArena.Server.Services.DBServices
{
    public class FriendService : IFriendService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FriendService> _logger;

        public FriendService(IUnitOfWork unitOfWork, ILogger<FriendService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<Friend>> GetAllFriendsAsync(string playerId, CancellationToken cancellationToken)
        {
            return await _unitOfWork.FriendRepository.GetAllFriendsAsync(playerId, cancellationToken);
        }
        public async Task<List<Friend>> GetFriendshipFriends(string playerId, CancellationToken cancellationToken)
        {
            return await _unitOfWork.FriendRepository.GetFriendshipFriends(playerId, cancellationToken);
        }
        public async Task<Friend> GetFriendAsync(string playerId1, string playerId2, CancellationToken cancellationToken)
        {
            return await _unitOfWork.FriendRepository.GetFriendAsync(playerId1, playerId2, cancellationToken);
        }
        public async Task<Friend> GetFriendAsync(int idFriend, CancellationToken cancellationToken)
        {
            return await _unitOfWork.FriendRepository.GetFriendAsync(idFriend, cancellationToken);
        }

        public async Task<Result<Unit, ErrorResponse>> AddFriendAsync
            (string requesterId, string addresseeId, CancellationToken cancellationToken, bool commit = true)
        {
            try
            {
                var resultAdd = await _unitOfWork.FriendRepository.AddFriendAsync(requesterId, addresseeId, cancellationToken);
                if (!resultAdd.IsSuccess)
                    return resultAdd;

                if (commit)
                    await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding Friend");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Error adding Friend"
                });
            }
        }

        public async Task<Result<Unit, ErrorResponse>> ApproveFriendshipAsync
            (string authUserId, int idFriend, CancellationToken cancellationToken, bool commit = true)
        {
            try
            {
                var friend = await GetFriendAsync(idFriend, cancellationToken);
                if(friend == null)
                    return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                    { Error = "Friend not found" });

                if(friend.AddresseeId != authUserId)
                    return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                    { Error = "You cannot complete the approval for this friendship" });

                await _unitOfWork.FriendRepository.ApproveFriendshipAsync(idFriend, cancellationToken);
                if (commit)
                    await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error update Friend");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Error update Friend"
                });
            }
        }

        public async Task<Result<Unit, ErrorResponse>> DeleteFriendAsync
            (string authUserId, int idFriend, CancellationToken cancellationToken, bool commit = true)
        {
            try
            {
                var friend = await GetFriendAsync(idFriend, cancellationToken);
                if (friend == null)
                    return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                    { Error = "Friend not found" });

                if (friend.AddresseeId != authUserId && friend.RequesterId != authUserId)
                    return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                    { Error = "You can't delete this friendship" });


                await _unitOfWork.FriendRepository.DeleteFriendAsync(idFriend, cancellationToken);
                if (commit)
                    await _unitOfWork.CommitAsync(cancellationToken);

                return Result.Success<Unit, ErrorResponse>(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Friend");
                return Result.Failure<Unit, ErrorResponse>(new ErrorResponse
                {
                    Error = "Error deleting Friend"
                });
            }
        }
    }
}
