using FluentValidation;

namespace CodeBattleArena.Application.Features.Notifications.Queries.GetNotificationsList
{
    public class GetNotificationsListValidator : AbstractValidator<GetNotificationsListQuery>
    {
        public GetNotificationsListValidator()
        {
            RuleFor(x => x.PlayerId).NotEmpty().WithMessage("PlayerId is required.");
        }
    }
}
