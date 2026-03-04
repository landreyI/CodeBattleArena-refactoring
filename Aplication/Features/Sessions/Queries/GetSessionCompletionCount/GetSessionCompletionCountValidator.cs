
using FluentValidation;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSessionCompletionCount
{
    public class GetSessionPlayersValidator : AbstractValidator<GetSessionCompletionCountQuery>
    {
        public GetSessionPlayersValidator()
        {
            RuleFor(x => x.SessionId).NotEmpty().WithMessage("SessionId is required.");
        }
    }
}
