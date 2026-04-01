using FluentValidation;

namespace CodeBattleArena.Application.Features.Leagues.Commands.DeleteLeague
{
    public class DeleteLeagueValidator : AbstractValidator<DeleteLeagueCommand>
    {
        public DeleteLeagueValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("LeagueId is required.");
        }
    }
}
