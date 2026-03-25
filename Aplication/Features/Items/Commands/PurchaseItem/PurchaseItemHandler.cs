
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Items.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Items;
using CodeBattleArena.Domain.PlayerItems;
using CodeBattleArena.Domain.Players;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Commands.PurchaseItem
{
    public class PurchaseItemHandler : IRequestHandler<PurchaseItemCommand, Result<bool>>
    {
        private readonly IRepository<Item> _itemRepository;
        private readonly IRepository<PlayerItem> _playerItemsRepository;
        private readonly IRepository<Player> _playerRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;
        public PurchaseItemHandler(
            IRepository<Item> itemRepository, 
            IRepository<PlayerItem> playerItemsRepository, 
            IRepository<Player> playerRepository,
            IIdentityService identityService, 
            IUnitOfWork unitOfWork)
        {
            _itemRepository = itemRepository;
            _playerItemsRepository = playerItemsRepository;
            _playerRepository = playerRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(PurchaseItemCommand request, CancellationToken cancellationToken)
        {
            var currentPlayerContext = await _identityService.GetUserContextAsync();
            if (!currentPlayerContext.PlayerId.HasValue)
                return Result<bool>.Failure(new Error("auth.unauthorized", "User not found in context", 401));

            var player = await _playerRepository.GetByIdAsync(currentPlayerContext.PlayerId.Value, ct: cancellationToken);
            if (player == null)
                return Result<bool>.Failure(new Error("player_payer.not_found", "Player payer not found", 404));

            if (request.PlayerId.HasValue && request.PlayerId.Value != player.Id)
            {
                var recipientExists = await _playerRepository.AnyAsync(p => p.Id == request.PlayerId, ct: cancellationToken);
                if (!recipientExists)
                    return Result<bool>.Failure(new Error("player_recipient.not_found", "Player recipient not found", 404));
            }

            var recipientId = request.PlayerId ?? player.Id;

            var spec = new ItemForUpdateSpec(request.ItemId);
            var item = await _itemRepository.GetBySpecAsync(spec, cancellationToken);
            if (item == null)
                return Result<bool>.Failure(new Error("item.not_found", "Item not found", 404));

            if (!currentPlayerContext.IsStaff)
            {
                var purchaseResult = player.BuyItem(item);
                if (purchaseResult.IsFailure)
                    return Result<bool>.Failure(purchaseResult.Error);
            }

            var playerItemResult = PlayerItem.Create(recipientId, item.Id, player.Id);
            if (playerItemResult.IsFailure)
                return Result<bool>.Failure(playerItemResult.Error);

            await _playerItemsRepository.AddAsync(playerItemResult.Value, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
