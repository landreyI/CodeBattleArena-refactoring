
using FluentValidation;

namespace CodeBattleArena.Application.Features.Leagues.Queries.GetLeague
{
    public class GetLeagueValidator : AbstractValidator<GetLeagueQuery>
    {
        public GetLeagueValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("LeagueId is required.");
        }
    }
}
