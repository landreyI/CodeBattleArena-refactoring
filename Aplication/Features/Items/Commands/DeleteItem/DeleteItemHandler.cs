
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Items.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Items;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Commands.DeleteItem
{
    public class DeleteItemHandler : IRequestHandler<DeleteItemCommand, Result<bool>>
    {
        private readonly IRepository<Item> _itemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteItemHandler(
            IRepository<Item> itemRepository,
            IUnitOfWork unitOfWork)
        {
            _itemRepository = itemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _itemRepository.GetBySpecAsync(new ItemForUpdateSpec(request.Id));
            if (item == null)
                return Result<bool>.Failure(new Error("item.not_found", "Selected item not found", 404));

            item.Delete();
            _itemRepository.Remove(item);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
