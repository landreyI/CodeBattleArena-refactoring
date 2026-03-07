using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.ProgrammingTasks.Filters;
using CodeBattleArena.Application.Features.ProgrammingTasks.Queries.GetPlayerProgrammingTasksList;
using CodeBattleArena.Application.Features.Sessions.Filters;
using CodeBattleArena.Application.Features.Sessions.Queries.GetPlayerSessionHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Api.Controllers
{
    [Route("api/players")]
    [ApiController]
    public class PlayerController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;

        public PlayerController(IMediator mediator, IIdentityService identityService)
        {
            _mediator = mediator;
            _identityService = identityService;
        }

        [Authorize]
        [HttpGet("{id:guid}/sessions")]
        public async Task<IActionResult> GetPlayerSessionHistory([FromRoute] Guid id, [FromQuery] SessionFilter filter, CancellationToken ct)
            => HandleResult(await _mediator.Send(new GetPlayerSessionHistoryQuery(id, filter), ct));

        [HttpGet("{id:guid}/programming-tasks")]
        public async Task<IActionResult> GetPlayerTasks([FromRoute] Guid id, [FromQuery] ProgrammingTaskFilter filter, CancellationToken ct)
            => HandleResult(await _mediator.Send(new GetPlayerProgrammingTasksListQuery(id, filter), ct));
    }
}
