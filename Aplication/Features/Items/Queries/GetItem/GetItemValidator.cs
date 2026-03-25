
using FluentValidation;

namespace CodeBattleArena.Application.Features.Items.Queries.GetItem
{
    public class GetItemValidator : AbstractValidator<GetItemQuery>
    {
        public GetItemValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("ItemId is required.");
        }
    }
}
