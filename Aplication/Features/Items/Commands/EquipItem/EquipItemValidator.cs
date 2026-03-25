
using FluentValidation;

namespace CodeBattleArena.Application.Features.Items.Commands.EquipItem
{
    public class EquipItemValidator : AbstractValidator<EquipItemCommand>
    {
        public EquipItemValidator()
        {
            RuleFor(x => x.ItemId).NotEmpty().WithMessage("ItemId is required.");
        }
    }
}
