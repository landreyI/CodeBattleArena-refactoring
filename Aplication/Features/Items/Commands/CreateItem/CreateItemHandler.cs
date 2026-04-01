
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Items;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Commands.CreateItem
{
    public class CreateItemHandler : IRequestHandler<CreateItemCommand, Result<Guid>>
    {
        private readonly IRepository<Item> _itemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateItemHandler(IRepository<Item> itemRepository, IUnitOfWork unitOfWork)
        {
            _itemRepository = itemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            var resultItem = Item.Create(
                request.Name,
                request.Type,
                request.PriceCoin,
                request.ImageUrl,
                request.CssClass,
                request.Description);

            if (resultItem.IsFailure)
                return Result<Guid>.Failure(resultItem.Error);

            await _itemRepository.AddAsync(resultItem.Value, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid>.Success(resultItem.Value.Id);
        }
    }
}
