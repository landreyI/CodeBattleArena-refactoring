
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.Notifications;
using CodeBattleArena.Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CodeBattleArena.Infrastructure.SignalR.Services
{
    public class NotificationService : INotificationService
    {

        private readonly IRepository<Notification> _notificationRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IHubContext<MainHub> _hubContext;

        public NotificationService(
            IRepository<Notification> notificationRepo,
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            IHubContext<MainHub> hubContext)
        {
            _notificationRepo = notificationRepo;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _hubContext = hubContext;
        }
        public async Task<Result> SendNotificationAsync(Guid userId, string content, NotificationType type, Guid? relatedEntityId = default, CancellationToken ct = default)
        {
            var notificationResult = Notification.Create(userId, content, type);
            if (notificationResult.IsFailure) 
                return notificationResult;

            await _notificationRepo.AddAsync(notificationResult.Value, ct);
            await _unitOfWork.CommitAsync(ct);

            bool isOnline = await _cacheService.ExistsAsync("online_user:" + userId, ct);

            if (isOnline)
            {
                await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", new
                {
                    Id = notificationResult.Value.Id,
                    Content = notificationResult.Value.Content,
                    Type = notificationResult.Value.Type.ToString(),
                    RelatedEntityId = relatedEntityId,
                    CreatedAt = notificationResult.Value.CreatedAt
                }, ct);
            }

            return Result.Success();
        }
    }
}
