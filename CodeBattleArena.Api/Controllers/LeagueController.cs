using CodeBattleArena.Application.Features.Leagues.Queries.GetLeague;
using CodeBattleArena.Application.Features.Leagues.Queries.GetLeaguesList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Api.Controllers
{
    [Route("api/leagues")]
    [ApiController]
    public class LeagueController : BaseApiController
    {
        private readonly IMediator _mediator;
        public LeagueController(IMediator mediator) { _mediator = mediator; }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
            => HandleResult(await _mediator.Send(new GetLeagueQuery(id), ct));

        [HttpGet]
        public async Task<IActionResult> GetItemsList(CancellationToken ct)
            => HandleResult(await _mediator.Send(new GetLeaguesListQuery(), ct));
    }
}
