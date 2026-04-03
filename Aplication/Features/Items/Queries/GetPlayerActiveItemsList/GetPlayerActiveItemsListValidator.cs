using FluentValidation;

namespace CodeBattleArena.Application.Features.Items.Queries.GetPlayerActiveItemsList
{
    public class GetPlayerActiveItemsListValidator : AbstractValidator<GetPlayerActiveItemsListQuery>
    {
        public GetPlayerActiveItemsListValidator()
        {
            RuleFor(x => x.PlayerId).NotEmpty().WithMessage("PlayerId is required.");
        }
    }
}
