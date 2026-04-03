
using FluentValidation;

namespace CodeBattleArena.Application.Features.Players.Queries.GetPlayer
{
    public class GetPlayerValidator : AbstractValidator<GetPlayerQuery>
    {
        public GetPlayerValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("PlayerId is required.");
        }
    }
}
