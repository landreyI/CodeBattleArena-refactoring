
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Items.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Items;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Commands.UpdateItem
{
    public class UpdateItemHandler : IRequestHandler<UpdateItemCommand, Result<bool>>
    {
        private readonly IRepository<Item> _itemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateItemHandler(IRepository<Item> itemRepository, IUnitOfWork unitOfWork)
        {
            _itemRepository = itemRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _itemRepository.GetBySpecAsync(new ItemForUpdateSpec(request.Id));
            if (item == null)
                return Result<bool>.Failure(new Error("item.not_found", "Selected item not found", 404));

            var resultUpdate = item.Update(
                request.Name,
                request.Type,
                request.PriceCoin,
                request.ImageUrl,
                request.CssClass,
                request.Description);

            if (resultUpdate.IsFailure)
                return Result<bool>.Failure(resultUpdate.Error);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
