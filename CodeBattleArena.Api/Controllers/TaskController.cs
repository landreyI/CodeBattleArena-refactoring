using CodeBattleArena.Application.Features.ProgrammingTasks.Commands.RequestAIProgrammingTask;
using CodeBattleArena.Application.Features.ProgrammingTasks.Filters;
using CodeBattleArena.Application.Features.ProgrammingTasks.Queries.GetProgrammingTask;
using CodeBattleArena.Application.Features.ProgrammingTasks.Queries.GetProgrammingTasksList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Api.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TaskController : BaseApiController
    {
        private readonly IMediator _mediator;

        public TaskController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
            => HandleResult(await _mediator.Send(new GetProgrammingTaskQuery(id), ct));

        [HttpGet]
        public async Task<IActionResult> GetTasksList([FromQuery] ProgrammingTaskFilter? filter, CancellationToken ct)
            => HandleResult(await _mediator.Send(new GetProgrammingTasksListQuery(filter), ct));

        [Authorize]
        [HttpPost("generate-ai")]
        public async Task<IActionResult> GenerateAI([FromBody] RequestAIProgrammingTaskCommand command, CancellationToken cancellationToken)
            => HandleResult(await _mediator.Send(command, cancellationToken));
    }
}
