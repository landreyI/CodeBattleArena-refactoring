
using FluentValidation;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSessionPlayers
{
    public class GetSessionPlayersValidator : AbstractValidator<GetSessionPlayersQuery>
    {
        public GetSessionPlayersValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("SessionId is required.");
        }
    }
}
