using AutoMapper;
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Items.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Notifications;
using MediatR;


namespace CodeBattleArena.Application.Features.Notifications.Queries.GetNotificationsList
{
    public class GetNotificationsListHandler
        : IRequestHandler<GetNotificationsListQuery, Result<PaginatedResult<NotificationDto>>>
    {
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IMapper _mapper;
        public GetNotificationsListHandler(IRepository<Notification> notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }
        public async Task<Result<PaginatedResult<NotificationDto>>> Handle(GetNotificationsListQuery request, CancellationToken cancellationToken)
        {
            var spec = new NotificationsListSpec(request.PlayerId, request.Filter);
            var notifications = await _notificationRepository.GetListBySpecAsync(spec, cancellationToken);
            var totalCount = await _notificationRepository.CountAsync(spec, cancellationToken);

            var dtos = _mapper.Map<List<NotificationDto>>(notifications);

            var result = new PaginatedResult<NotificationDto>(
                dtos,
                totalCount,
                request.Filter?.PageNumber ?? 1,
                request.Filter?.PageSize ?? 15);

            return Result<PaginatedResult<NotificationDto>>.Success(result);
        }
    }
}
