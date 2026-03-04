using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Settings;
using CodeBattleArena.Application.Features.Auth.Models;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Players;
using MediatR;
using Microsoft.Extensions.Options;

namespace CodeBattleArena.Application.Features.Auth.Commands.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly IRepository<Player> _playerRepository;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly JwtSettings _jwtSettings;
        public LoginHandler(
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


        public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
        {
            var authData = await _identityService.AuthenticateAsync(request.Email, request.Password);
            if (authData == null)
                return Result<AuthResponse>.Failure(new Error("auth.invalid_credentials", "Incorrect email or password"));

            var player = await _playerRepository.GetFirstOrDefaultAsync(p => p.IdentityId == authData.Value.UserId, ct: ct);
            if (player is null)
                return Result<AuthResponse>.Failure(new Error("players.not_found", "Player not found", 404));

            var newAccessToken = _tokenService.GenerateAccessToken(player.IdentityId, authData.Value.Email, player.Id, authData.Value.Roles);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            player.UpdateRefreshToken(newRefreshToken, _jwtSettings.RefreshTokenExpiryDays);

            await _unitOfWork.CommitAsync(ct);

            return Result<AuthResponse>.Success(new AuthResponse { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
        }
    }
}
