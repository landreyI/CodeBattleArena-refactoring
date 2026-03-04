using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services;
using System.Threading.Tasks;
using Travel_Agency.Service;
using CodeBattleArena.Server.Helpers;
using CodeBattleArena.Server.Untils;

namespace Travel_Agency.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleAuthController : ControllerBase
    {
        private readonly UserManager<Player> _userManager;
        private readonly SignInManager<Player> _signInManager;
        private readonly GoogleAuthService _googleAuthService;

        public GoogleAuthController(
            UserManager<Player> userManager,
            SignInManager<Player> signInManager,
            GoogleAuthService googleAuthService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _googleAuthService = googleAuthService;
        }

        [HttpGet("google-redirect-oauth-server")]
        public IActionResult RedirectOnOauthServer()
        {
            var url = _googleAuthService.GenerateOauthRequestUrl();
            return Ok(new { url });
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> OauthCallback(string code, string? error, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(error))
                return BadRequest(new ErrorResponse { Error = $"Google Authorization Error: {error}" });

            var tokenResponse = await _googleAuthService.ExchangeCodeOnTokenAsync(code, cancellationToken);
            if (tokenResponse == null)
                return BadRequest(new ErrorResponse { Error = "Failed to obtain access token." });

            var infoUser = await _googleAuthService.GetUserInfoAsync(tokenResponse.AccessToken, cancellationToken);
            if (infoUser == null)
                return BadRequest(new ErrorResponse { Error = "Failed to get user info." });

            var player = await _userManager.FindByEmailAsync(infoUser.Email);

            if (player == null)
            {
                player = new Player
                {
                    Email = infoUser.Email,
                    UserName = "player" + new Random().Next(0, 10000000).ToString("D6"),
                    PhotoUrl = infoUser.Picture
                };

                var result = await _userManager.CreateAsync(player);
                if (!result.Succeeded)
                    return BadRequest(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });

                await _userManager.AddLoginAsync(player, new ExternalLoginInfo(null, "Google", infoUser.Id, null));
                await _userManager.AddToRolesAsync(player, ["User"]);
            }

            var loginInfo = new ExternalLoginInfo(null, "Google", infoUser.Id, null);
            var signInResult = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                // Успешный вход, куки уже установлены
            }
            else if (signInResult.IsNotAllowed || signInResult.IsLockedOut)
            {
                return BadRequest(new ErrorResponse { Error = "Login not allowed or user is locked out." });
            }
            else
            {
                await _userManager.AddLoginAsync(player, loginInfo);
                await _signInManager.SignInAsync(player, isPersistent: true);
            }

            var redirectUrl = "https://localhost:55689/google-oauth-success";
            return Redirect(redirectUrl);
        }
    }
}