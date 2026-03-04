
using FluentValidation;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSession
{
    public class GetSessionPlayersValidator : AbstractValidator<GetSessionQuery>
    {
        public GetSessionPlayersValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("SessionId is required.");
        }
    }
}
