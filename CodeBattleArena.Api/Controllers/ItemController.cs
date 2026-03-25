
using Amazon.S3.Model;
using CodeBattleArena.Application.Features.Items.Commands.PurchaseItem;
using CodeBattleArena.Application.Features.Items.Filters;
using CodeBattleArena.Application.Features.Items.Queries.GetItem;
using CodeBattleArena.Application.Features.Items.Queries.GetItemsList;
using CodeBattleArena.Application.Features.Items.Queries.GetPlayerItem;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Api.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemController : BaseApiController
    {
        private readonly IMediator _mediator;
        public ItemController(IMediator mediator) { _mediator = mediator; }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
            => HandleResult(await _mediator.Send(new GetItemQuery(id), ct));

        [HttpGet]
        public async Task<IActionResult> GetItemsList([FromQuery] ItemFilter filter, CancellationToken ct)
            => HandleResult(await _mediator.Send(new GetItemsListQuery(filter), ct));

        [HttpGet("players/{playerId:guid}/items/{itemId:guid}")]
        public async Task<IActionResult> GetPlayerItem([FromRoute] Guid playerId, [FromRoute] Guid itemId, CancellationToken ct)
            => HandleResult(await _mediator.Send(new GetPlayerItemQuery(playerId, itemId), ct));

        [Authorize]
        [HttpPost("{itemId:guid}/purchase")]
        [HttpPost("{itemId:guid}/purchase-for/{playerId:guid}")] // For friend
        public async Task<IActionResult> PurchaseItem([FromRoute] Guid itemId, [FromRoute] Guid? playerId, CancellationToken ct)
            => HandleResult(await _mediator.Send(new PurchaseItemCommand(itemId, playerId), ct));
    }
}
