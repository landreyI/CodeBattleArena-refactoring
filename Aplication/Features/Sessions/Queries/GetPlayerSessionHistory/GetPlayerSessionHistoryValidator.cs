using FluentValidation;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetPlayerSessionHistory
{
    public class GetPlayerSessionHistoryValidator : AbstractValidator<GetPlayerSessionHistoryQuery>
    {
        public GetPlayerSessionHistoryValidator()
        {
            RuleFor(x => x.PlayerId).NotEmpty().WithMessage("PlayerId is required.");
        }
    }
}
