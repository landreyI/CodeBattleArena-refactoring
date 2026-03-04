using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Settings;
using CodeBattleArena.Application.Features.Auth.Models;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Players;
using MediatR;
using Microsoft.Extensions.Options;

namespace CodeBattleArena.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
    {
        private readonly IRepository<Player> _playerRepository;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly JwtSettings _jwtSettings;

        public RefreshTokenHandler(
            IRepository<Player> playerRepository, 
            ITokenService tokenService, 
            IUnitOfWork unitOfWork, 
            IIdentityService identityService,
            IOptions<JwtSettings> jwtOptions)
        {
            _playerRepository = playerRepository;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
        {
            var player = await _playerRepository.GetFirstOrDefaultAsync(p => p.RefreshToken == request.refreshToken, ct: ct);
            if(player is null || player.RefreshTokenExpiryTime < DateTime.UtcNow) 
                return Result<AuthResponse>.Failure(new Error("refresh_token", "Refresh token is invalid or expired."));

            var userEmail = await _identityService.GetUserEmailByIdAsync(player.IdentityId);
            var roles = await _identityService.GetUserRolesAsync(player.IdentityId);

            var newAccessToken = _tokenService.GenerateAccessToken(player.IdentityId, userEmail, player.Id, roles);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            player.UpdateRefreshToken(newRefreshToken, _jwtSettings.RefreshTokenExpiryDays);

            await _unitOfWork.CommitAsync(ct);

            return Result<AuthResponse>.Success(new AuthResponse { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
        }
    }
}
