
using FluentValidation;

namespace CodeBattleArena.Application.Features.Items.Commands.PurchaseItem
{
    public class PurchaseItemValidator : AbstractValidator<PurchaseItemCommand>
    {
        public PurchaseItemValidator()
        {
            RuleFor(x => x.ItemId).NotEmpty().WithMessage("ItemId is required.");
        }
    }
}
