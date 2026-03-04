using CodeBattleArena.Application.Common.Events;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Settings;
using CodeBattleArena.Application.Features.Auth.Models;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Players;
using MediatR;
using Microsoft.Extensions.Options;

namespace CodeBattleArena.Application.Features.Auth.Commands.GoogleLogin
{
    public class GoogleLoginHandler : IRequestHandler<GoogleLoginCommand, Result<AuthResponse>>
    {
        private readonly IGoogleAuthService _googleService;
        private readonly IIdentityService _identityService;
        private readonly IRepository<Player> _playerRepository;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly JwtSettings _jwtSettings;

        public GoogleLoginHandler(
            IGoogleAuthService googleService,
            IIdentityService identityService,
            IRepository<Player> playerRepository,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            IMediator mediator,
            IOptions<JwtSettings> jwtOptions)
        {
            _googleService = googleService;
            _identityService = identityService;
            _playerRepository = playerRepository;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<Result<AuthResponse>> Handle(GoogleLoginCommand request, CancellationToken ct)
        {
            // 1. Обмен кода на данные пользователя Google
            var tokenResponse = await _googleService.ExchangeCodeOnTokenAsync(request.Code, ct);
            if (tokenResponse == null)
                return Result<AuthResponse>.Failure(new Error("google.auth", "Failed to obtain Google access token.", 401));

            var googleUser = await _googleService.GetUserInfoAsync(tokenResponse.AccessToken, ct);
            if (googleUser == null)
                return Result<AuthResponse>.Failure(new Error("google.user_info", "Failed to get Google user info.", 401));
            

            var (userId, succeeded) = await _identityService.GetOrCreateExternalUserAsync(
                googleUser.Email, "Google", googleUser.Id, googleUser.Name, googleUser.Picture);

            if (!succeeded)
                return Result<AuthResponse>.Failure(new Error("identity.sync", "External auth sync failed.", 401));

            var player = await _playerRepository.GetFirstOrDefaultAsync(p => p.IdentityId == userId, ct:ct);

            if (player == null)
            {
                var playerResult = Player.Create(userId, googleUser.Name);
                if (playerResult.IsFailure) return Result<AuthResponse>.Failure(playerResult.Error);

                player = playerResult.Value;
                var updateResult = player.UpdateProfile(googleUser.Name, googleUser.Picture);
                if (updateResult.IsFailure) return Result<AuthResponse>.Failure(updateResult.Error);

                await _playerRepository.AddAsync(player, ct);
            }

            var roles = await _identityService.GetUserRolesAsync(userId);
            var accessToken = _tokenService.GenerateAccessToken(userId, googleUser.Email, player.Id, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();

            player.UpdateRefreshToken(refreshToken, _jwtSettings.RefreshTokenExpiryDays);
            await _unitOfWork.CommitAsync(ct);

            await _mediator.Publish(new UserRegisteredEvent(player.Id, googleUser.Email, player.Profile.Name), ct);

            return Result<AuthResponse>.Success(new AuthResponse { AccessToken = accessToken, RefreshToken = refreshToken });
        }
    }
}
