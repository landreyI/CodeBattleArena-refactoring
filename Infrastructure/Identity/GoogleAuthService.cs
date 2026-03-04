using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Auth.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CodeBattleArena.Infrastructure.Identity
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GoogleAuthService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _configuration = config;
        }

        /// <summary>
        /// Генерирует URL для перенаправления на сервер OAuth Google.
        /// </summary>
        public string GenerateOauthRequestUrl()
        {
            var oAuthServerEndpoint = _configuration["GoogleOAuth:OauthUri"]
                ?? throw new InvalidOperationException("OAuth URI is not configured in the configuration.");

            var queryParams = new Dictionary<string, string>
            {
                { "client_id", _configuration["GoogleOAuth:ClientId"] },
                { "redirect_uri", _configuration["GoogleOAuth:RedirectUri"] },
                { "response_type", "code" },
                { "scope", "https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile" },
                { "access_type", "offline" },
                { "state", Guid.NewGuid().ToString() }
            };

            var url = QueryHelpers.AddQueryString(oAuthServerEndpoint, queryParams);

            return url;
        }

        /// <summary>
        /// Обменивает код авторизации на токен доступа.
        /// </summary>
        public async Task<GoogleTokenResponse?> ExchangeCodeOnTokenAsync(string code, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("The authorization code cannot be empty.", nameof(code));

            var tokenEndpoint = _configuration["GoogleOAuth:TokenEndpoint"]
                ?? throw new InvalidOperationException("Token Endpoint is not configured in the configuration.");

            var authParams = new Dictionary<string, string>
            {
                { "client_id", _configuration["GoogleOAuth:ClientId"] },
                { "client_secret", _configuration["GoogleOAuth:ClientSecret"] },
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", _configuration["GoogleOAuth:RedirectUri"] }
            };

            // Логируем параметры запроса

            using var content = new FormUrlEncodedContent(authParams);
            using var response = await _httpClient.PostAsync(tokenEndpoint, content, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);


            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Request failed. Google Token endpoint: {response.StatusCode}. Details: {responseContent}");
            }

            // Используем Newtonsoft.Json для десериализации, так как вы подключили его
            var tokenResponse = JsonConvert.DeserializeObject<GoogleTokenResponse>(responseContent);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new Exception($"Unable to process response from Google OAuth: Token missing. Full response: {responseContent}");
            }

            Console.WriteLine($"Access Token received: {tokenResponse.AccessToken}, expires in: {tokenResponse.ExpiresIn} Second");
            return tokenResponse;
        }

        /// <summary>
        /// Получает информацию о пользователе с использованием токена доступа.
        /// </summary>
        public async Task<GoogleUserInfoResponse?> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentException("The access token cannot be empty.", nameof(accessToken));

            var userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";

            using var request = new HttpRequestMessage(HttpMethod.Get, userInfoEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


            using var response = await _httpClient.SendAsync(request, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);


            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Google UserInfo API request error: {response.StatusCode}. Details: {responseContent}");
            }

            // Используем Newtonsoft.Json для десериализации
            var userInfo = JsonConvert.DeserializeObject<GoogleUserInfoResponse>(responseContent);

            if (userInfo == null)
            {
                throw new Exception($"Unable to process response from Google UserInfo: No data available. Full response: {responseContent}");
            }

            return userInfo;
        }
    }
}