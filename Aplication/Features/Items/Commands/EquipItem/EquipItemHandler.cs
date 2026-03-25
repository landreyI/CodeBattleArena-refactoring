
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Items.Specifications;
using CodeBattleArena.Application.Features.Players.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.PlayerItems;
using CodeBattleArena.Domain.Players;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Commands.EquipItem
{
    public class EquipItemHandler : IRequestHandler<EquipItemCommand, Result<bool>>
    {
        private readonly IRepository<PlayerItem> _playerItemsRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;
        public EquipItemHandler(
            IRepository<PlayerItem> playerItemsRepository,
            IIdentityService identityService,
            IUnitOfWork unitOfWork)
        {
            _playerItemsRepository = playerItemsRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(EquipItemCommand request, CancellationToken cancellationToken)
        {
            var currentPlayerContext = await _identityService.GetUserContextAsync();
            if (!currentPlayerContext.PlayerId.HasValue)
                return Result<bool>.Failure(new Error("auth.unauthorized", "User not found in context", 401));

            var specPlayerItem = new PlayerItemForUpdateSpec(currentPlayerContext.PlayerId.Value, request.ItemId);
            var playerItem = await _playerItemsRepository.GetBySpecAsync(specPlayerItem, cancellationToken);

            if (playerItem == null)
                return Result<bool>.Failure(new Error("palyer_item.not_found", "PlayerItem not found", 404));

            var result = playerItem.Player!.EquipItem(playerItem);
            if (result.IsFailure)
                return Result<bool>.Failure(result.Error);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
