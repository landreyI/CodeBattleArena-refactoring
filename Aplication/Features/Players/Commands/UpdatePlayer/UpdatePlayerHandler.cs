
using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Players.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Players;
using MediatR;

namespace CodeBattleArena.Application.Features.Players.Commands.UpdatePlayer
{
    public class UpdatePlayerHandler : IRequestHandler<UpdatePlayerCommand, Result<bool>>
    {
        private readonly IRepository<Player> _playerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        public UpdatePlayerHandler(IRepository<Player> playerRepository, IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _playerRepository = playerRepository;
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        public async Task<Result<bool>> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
        {
            var userContext = await _identityService.GetUserContextAsync();

            if (!userContext.PlayerId.HasValue)
                return Result<bool>.Failure(new Error("auth.unauthorized", "User not found in context", 401));

            if (!userContext.IsModerator && userContext.PlayerId != request.Id)
                return Result<bool>.Failure(new Error("auth.forbidden", "You are not allowed to change information about other players.", 403));

            var player = await _playerRepository.GetBySpecAsync(new PlayerForUpdateSpec(request.Id), cancellationToken);
            if (player == null)
                return Result<bool>.Failure(new Error("player.not_found", "Selected player not found", 404));

            var resultUpdate = player.UpdateProfile(
                request.Name,
                request.PhotoUrl,
                request.Info);

            if (resultUpdate.IsFailure)
                return Result<bool>.Failure(resultUpdate.Error);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
