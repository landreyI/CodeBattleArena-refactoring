using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Auth.Models;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Players;
using MediatR;

namespace CodeBattleArena.Application.Features.Auth.Queries.GetCurrentUser
{
    public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, Result<UserAuthDto?>>
    {
        private readonly IRepository<Player> _playerRepository;
        private readonly IIdentityService _identityService;

        public GetCurrentUserHandler(
            IRepository<Player> playerRepository,
            IIdentityService identityService)
        {
            _playerRepository = playerRepository;
            _identityService = identityService;
        }

        public async Task<Result<UserAuthDto?>> Handle(GetCurrentUserQuery request, CancellationToken ct)
        {
            var identityId = _identityService.CurrentIdentityId();
            var playerId = _identityService.CurrentPlayerId();

            if (identityId == null || playerId == null)
                return Result<UserAuthDto?>.Failure(new Error("auth.unauthorized", "User not found in context", 401));

            var player = await _playerRepository.GetByIdAsync(playerId.Value, ct: ct);
            if (player is null)
                return Result<UserAuthDto?>.Failure(new Error("players.not_found", "Player not found", 404));

            var roles = await _identityService.GetUserRolesAsync(identityId);

            return Result<UserAuthDto?>.Success(new UserAuthDto(
                player.IdentityId,
                player.Profile.Name,
                player.Profile.PhotoUrl,
                player.Wallet.Coins,
                player.Stats.Experience,
                roles));
        }
    }
}