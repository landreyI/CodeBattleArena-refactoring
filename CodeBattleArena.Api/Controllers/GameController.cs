using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Game.Commands.CheckCode;
using CodeBattleArena.Application.Features.Game.Commands.FinishTask;
using CodeBattleArena.Application.Features.Game.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Api.Controllers
{
    [Route("api/games")]
    [ApiController]
    public class GameController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;

        public GameController(IMediator mediator, IIdentityService identityService)
        {
            _mediator = mediator;
            _identityService = identityService;
        }

        [Authorize]
        [HttpPost("check-code")]
        public async Task<IActionResult> CheckCode([FromBody] CodeRequest codeRequest, CancellationToken cancellationToken)
            => HandleResult(await _mediator.Send(new CheckCodeCommand(codeRequest.Code), cancellationToken));

        [Authorize]
        [HttpPut("finish-task")]
        public async Task<IActionResult> FinistTask(CancellationToken cancellationToken)
            => HandleResult(await _mediator.Send(new FinishTaskCommand(), cancellationToken));
    }
}
