using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Auth.Commands.GoogleLogin;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleAuthController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IGoogleAuthService _googleService;

        public GoogleAuthController(IMediator mediator, IGoogleAuthService googleService)
        {
            _mediator = mediator;
            _googleService = googleService;
        }

        [HttpGet("google-url")]
        public IActionResult GetGoogleUrl() => Ok(new { url = _googleService.GenerateOauthRequestUrl() });

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback(string code)
        {
            var result = await _mediator.Send(new GoogleLoginCommand(code));

            const string frontendUrl = "https://localhost:55689/google-oauth-success";

            if (result.IsFailure)
            {
                return Redirect($"{frontendUrl}-error?message={result.Error.Message}");
            }

            var response = result.Value;
            var redirectUrl = $"{frontendUrl}?accessToken={response.AccessToken}&refreshToken={response.RefreshToken}";

            return Redirect(redirectUrl);
        }
    }
}
