
using CodeBattleArena.Application.Features.ProgrammingLanguages.Queries.GetLanguagesList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Api.Controllers
{
    [ApiController]
    [Route("api/programming-languages")]
    public class ProgrammingLanguageController : BaseApiController
    {
        private readonly IMediator _mediator;
        public ProgrammingLanguageController(IMediator mediator) { _mediator = mediator; }

        [HttpGet]
        public async Task<IActionResult> GetLanguagesList(CancellationToken ct)
            => HandleResult(await _mediator.Send(new GetLanguagesListQuery(), ct));
    }
}
