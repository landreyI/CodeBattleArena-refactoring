using CodeBattleArena.Application.Features.Auth.Commands.Login;
using CodeBattleArena.Application.Features.Auth.Commands.RefreshToken;
using CodeBattleArena.Application.Features.Auth.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CodeBattleArena.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseApiController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        => HandleResult(await _mediator.Send(command));

        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetCurrentUser()
            => HandleResult(await _mediator.Send(new GetCurrentUserQuery()));

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
            => HandleResult(await _mediator.Send(command));

        [HttpPost("logout")]
        public async Task<IActionResult> Logout() => Ok();
    }
}
