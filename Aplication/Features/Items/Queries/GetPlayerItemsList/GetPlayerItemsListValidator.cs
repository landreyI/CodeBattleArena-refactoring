using FluentValidation;

namespace CodeBattleArena.Application.Features.Items.Queries.GetPlayerItemsList
{
    public class GetPlayerItemsListValidator : AbstractValidator<GetPlayerItemsListQuery>
    {
        public GetPlayerItemsListValidator()
        {
            RuleFor(x => x.PlayerId).NotEmpty().WithMessage("PlayerId is required.");
        }
    }
}
