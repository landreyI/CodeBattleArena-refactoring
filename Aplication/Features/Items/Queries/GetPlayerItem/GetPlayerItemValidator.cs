
using FluentValidation;

namespace CodeBattleArena.Application.Features.Items.Queries.GetPlayerItem
{
    public class GetPlayerItemValidator : AbstractValidator<GetPlayerItemQuery>
    {
        public GetPlayerItemValidator()
        {
            RuleFor(x => x.PlayerId).NotEmpty().WithMessage("PlayerId is required.");
            RuleFor(x => x.ItemId).NotEmpty().WithMessage("ItemId is required.");
        }
    }
}
