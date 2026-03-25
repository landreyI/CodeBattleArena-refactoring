
using CodeBattleArena.Application.Features.Items.Commands.CreateItem;
using CodeBattleArena.Application.Features.Items.Commands.DeleteItem;
using CodeBattleArena.Application.Features.Items.Commands.UpdateItem;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Api.Controllers
{
    [ApiController]
    [Authorize(Policy = "CanEditPolicy")]
    [Route("api/admin/items")]
    public class AdminItemsController : BaseApiController
    {
        private readonly IMediator _mediator;

        public AdminItemsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost] // надо добавить проверку на то что ссылка из S3
        public async Task<IActionResult> Create([FromForm] CreateItemCommand command, CancellationToken ct)
            => HandleResult(await _mediator.Send(command, ct));

        [HttpPut("{id:guid}")] // надо добавить проверку на то что ссылка из S3
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromBody] UpdateItemCommand command, CancellationToken ct)
        {
            if (id != command.Id) return BadRequest("ID mismatch");
            return HandleResult(await _mediator.Send(command, ct));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
            => HandleResult(await _mediator.Send(new DeleteItemCommand(id), cancellationToken));
    }
}
