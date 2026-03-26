
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Items.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Notifications;
using MediatR;

namespace CodeBattleArena.Application.Features.Notifications.Queries.GetNotification
{
    public class GetNotificationHandler : IRequestHandler<GetNotificationQuery, Result<Notification>>
    {
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        public GetNotificationHandler(IRepository<Notification> notificationRepository, IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }
        public async Task<Result<Notification>> Handle(GetNotificationQuery request, CancellationToken cancellationToken)
        {
            var currentPlayerContext = await _identityService.GetUserContextAsync();
            if (!currentPlayerContext.PlayerId.HasValue)
                return Result<Notification>.Failure(new Error("auth.unauthorized", "User not found in context", 401));

            var notifi = await _notificationRepository.GetBySpecAsync(new NotificationForUpdateSpec(request.Id), cancellationToken);
            if (notifi == null)
                return Result<Notification>.Failure(new Error("notification.not_found", "Notification not found", 404));

            if(currentPlayerContext.PlayerId != notifi.UserId)
                return Result<Notification>.Failure(new Error("Forbidden", "Not your notification", 403));

            if (notifi.IsRead)
                return Result<Notification>.Success(notifi).WithoutModification(); //Отменяем удаление кеша

            notifi.MarkAsRead();
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Notification>.Success(notifi);
        }
    }
}
