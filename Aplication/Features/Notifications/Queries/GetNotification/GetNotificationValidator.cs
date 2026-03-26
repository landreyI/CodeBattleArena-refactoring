using FluentValidation;

namespace CodeBattleArena.Application.Features.Notifications.Queries.GetNotification
{
    public class GetNotificationValidator : AbstractValidator<GetNotificationQuery>
    {
        public GetNotificationValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("NotificationId is required.");
        }
    }
}
