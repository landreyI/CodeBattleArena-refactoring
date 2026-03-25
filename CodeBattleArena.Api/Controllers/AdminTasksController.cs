using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.ProgrammingTasks.Commands.CreateProgrammingTask;
using CodeBattleArena.Application.Features.ProgrammingTasks.Commands.DeleteProgrammingTask;
using CodeBattleArena.Application.Features.ProgrammingTasks.Commands.UpdateProgrammingTask;
using CodeBattleArena.Application.Features.Sessions.Commands.DeleteSession;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Api.Controllers
{
    [ApiController]
    [Authorize(Policy = "CanEditPolicy")]
    [Route("api/admin/tasks")]
    public class AdminTasksController : BaseApiController
    {
        private readonly IMediator _mediator;

        public AdminTasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProgrammingTaskCommand command, CancellationToken ct)
            => HandleResult(await _mediator.Send(command, ct));

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromBody] UpdateProgrammingTaskCommand command, CancellationToken ct)
        {
            if (id != command.Id) return BadRequest("ID mismatch");
            return HandleResult(await _mediator.Send(command, ct));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
            => HandleResult(await _mediator.Send(new DeleteProgrammingTaskCommand(id), cancellationToken));
    }
}
