
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Notifications.Filters;
using CodeBattleArena.Application.Features.Notifications.Queries.GetNotification;
using CodeBattleArena.Application.Features.Notifications.Queries.GetNotificationsList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Api.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;

        public NotificationController(IMediator mediator, IIdentityService identityService)
        {
            _mediator = mediator;
            _identityService = identityService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
            => HandleResult(await _mediator.Send(new GetNotificationQuery(id)));

        [HttpGet]
        public async Task<IActionResult> GetNotificationsList([FromQuery] NotificationFilter filter, CancellationToken ct)
        {
            var myPlayerId = _identityService.CurrentPlayerId();
            if (!myPlayerId.HasValue) return Unauthorized();
            
            return HandleResult(await _mediator.Send(new GetNotificationsListQuery(myPlayerId.Value, filter), ct));
        }
    }
}
