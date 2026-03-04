
using FluentValidation;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSessionBestResult
{
    public class GetSessionBestResultValidator : AbstractValidator<GetSessionBestResultQuery>
    {
        public GetSessionBestResultValidator()
        {
            RuleFor(x => x.SessionId).NotEmpty().WithMessage("SessionId is required.");
        }
    }
}
