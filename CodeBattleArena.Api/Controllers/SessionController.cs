using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Sessions.Commands.CreateSession;
using CodeBattleArena.Application.Features.Sessions.Commands.DeleteSession;
using CodeBattleArena.Application.Features.Sessions.Commands.FinishGame;
using CodeBattleArena.Application.Features.Sessions.Commands.InviteSession;
using CodeBattleArena.Application.Features.Sessions.Commands.JoinSession;
using CodeBattleArena.Application.Features.Sessions.Commands.KickOutSession;
using CodeBattleArena.Application.Features.Sessions.Commands.StartGame;
using CodeBattleArena.Application.Features.Sessions.Commands.UnjoinSession;
using CodeBattleArena.Application.Features.Sessions.Commands.UpdateSession;
using CodeBattleArena.Application.Features.Sessions.Filters;
using CodeBattleArena.Application.Features.Sessions.Models;
using CodeBattleArena.Application.Features.Sessions.Queries.GetActiveSession;
using CodeBattleArena.Application.Features.Sessions.Queries.GetPlayerSessionInfo;
using CodeBattleArena.Application.Features.Sessions.Queries.GetSession;
using CodeBattleArena.Application.Features.Sessions.Queries.GetSessionBestResult;
using CodeBattleArena.Application.Features.Sessions.Queries.GetSessionCompletionCount;
using CodeBattleArena.Application.Features.Sessions.Queries.GetSessionPlayers;
using CodeBattleArena.Application.Features.Sessions.Queries.GetSessionsList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Api.Controllers
{
    [ApiController]
    [Route("api/sessions")]
    public class SessionController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;

        public SessionController(IMediator mediator, IIdentityService identityService)
        {
            _mediator = mediator;
            _identityService = identityService;
        }

        [Authorize]
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveSession(CancellationToken ct)
            => HandleResult(await _mediator.Send(new GetActiveSessionQuery(), ct));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
            => HandleResult(await _mediator.Send(new GetSessionQuery(id), ct));

        [HttpGet]
        public async Task<IActionResult> GetSessionsList([FromQuery] SessionFilter filter, CancellationToken ct)
            => HandleResult(await _mediator.Send(new GetSessionsListQuery(filter), ct));

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSessionCommand command, CancellationToken ct)
            => HandleResult(await _mediator.Send(command, ct));

        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromBody] UpdateSessionCommand command, CancellationToken ct)
        {
            if (id != command.Id) return BadRequest("ID mismatch");
            return HandleResult(await _mediator.Send(command, ct));
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
            => HandleResult(await _mediator.Send(new DeleteSessionCommand(id), cancellationToken));

        [HttpGet("{id:guid}/players")]
        public async Task<IActionResult> GetSessionPlayers([FromRoute] Guid id, CancellationToken ct)
            => HandleResult(await _mediator.Send(new GetSessionPlayersQuery(id), ct));

        [HttpGet("{sessionId:guid}/players/{playerId:guid}")]
        public async Task<IActionResult> GetPlayerSessionInfo([FromRoute] Guid sessionId, [FromRoute] Guid playerId, CancellationToken ct)
            => HandleResult(await _mediator.Send(new GetPlayerSessionInfoQuery(sessionId, playerId), ct));

        [Authorize]
        [HttpGet("{id:guid}/players/me")]
        public async Task<IActionResult> GetMyPlayerSessionInfo(Guid id, CancellationToken ct)
        {
            var myPlayerId = _identityService.CurrentPlayerId();
            if (!myPlayerId.HasValue) return Unauthorized();

            return HandleResult(await _mediator.Send(new GetPlayerSessionInfoQuery(id, myPlayerId.Value), ct));
        }

        [Authorize]
        [HttpPut("{id:guid}/join")]
        public async Task<IActionResult> JoinSession([FromRoute] Guid id, [FromBody] JoinSessionRequest request, CancellationToken ct)
            => HandleResult(await _mediator.Send(new JoinSessionCommand(id, request.Password), ct));

        [Authorize]
        [HttpPut("unjoin")]
        public async Task<IActionResult> UnjoinSession(CancellationToken ct)
            => HandleResult(await _mediator.Send(new UnjoinSessionCommand(), ct));

        [Authorize]
        [HttpPost("{id:guid}/invite")]
        public async Task<IActionResult> InviteSession([FromRoute] Guid id, [FromBody] List<Guid> idPlayersInvite, CancellationToken cancellationToken)
            => HandleResult(await _mediator.Send(new InviteSessionCommand(id, idPlayersInvite), cancellationToken));

        [Authorize]
        [HttpDelete("{sessionId:guid}/kick-out/{playerId:guid}")]
        public async Task<IActionResult> KickOut([FromRoute] Guid sessionId, [FromRoute] Guid playerId, CancellationToken cancellationToken)
            => HandleResult(await _mediator.Send(new KickOutSessionCommand(sessionId, playerId), cancellationToken));

        [Authorize]
        [HttpPut("{sessionId:guid}/select-task/{taskId:guid}")]
        public async Task<IActionResult> SelectTask([FromRoute] Guid sessionId, [FromRoute] Guid taskId, CancellationToken cancellationToken)
            => HandleResult(await _mediator.Send(new UpdateSessionCommand(sessionId, TaskId: taskId), cancellationToken));

        [Authorize]
        [HttpPut("{id:guid}/start-game")]
        public async Task<IActionResult> StartGame([FromRoute] Guid id, CancellationToken cancellationToken)
            => HandleResult(await _mediator.Send(new StartGameCommand(id), cancellationToken));

        [HttpGet("{id:guid}/completion-count")]
        public async Task<IActionResult> GetFinishedPlayersCount([FromRoute] Guid id, CancellationToken cancellationToken)
            => HandleResult(await _mediator.Send(new GetSessionCompletionCountQuery(id), cancellationToken));

        [Authorize]
        [HttpPut("{id:guid}/finish-game")]
        public async Task<IActionResult> FinishGame([FromRoute] Guid id, CancellationToken ct)
            => HandleResult(await _mediator.Send(new FinishGameCommand(id), ct));

        [HttpGet("{id:guid}/best-result")]
        public async Task<IActionResult> GetBestResult([FromRoute] Guid id, CancellationToken cancellationToken)
            => HandleResult(await _mediator.Send(new GetSessionBestResultQuery(id), cancellationToken));
    }
}
