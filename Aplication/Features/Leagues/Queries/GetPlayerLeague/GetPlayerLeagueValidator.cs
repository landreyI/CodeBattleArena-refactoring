
using FluentValidation;

namespace CodeBattleArena.Application.Features.Leagues.Queries.GetPlayerLeague
{
    public class GetPlayerLeagueValidator : AbstractValidator<GetPlayerLeagueQuery>
    {
        public GetPlayerLeagueValidator()
        {
            RuleFor(x => x.PlayerId).NotEmpty().WithMessage("PlayerId is required.");
        }
    }
}
