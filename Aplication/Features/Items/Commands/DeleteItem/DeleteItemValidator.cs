using FluentValidation;

namespace CodeBattleArena.Application.Features.Items.Commands.DeleteItem
{
    public class DeleteItemValidator : AbstractValidator<DeleteItemCommand>
    {
        public DeleteItemValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("ItemId is required.");
        }
    }
}
