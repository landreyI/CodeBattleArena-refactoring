using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Travel_Agency.Service
{
    public class GoogleAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GoogleAuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Генерирует URL для перенаправления на сервер OAuth Google.
        /// </summary>
        public string GenerateOauthRequestUrl()
        {
            var oAuthServerEndpoint = _configuration["GoogleOAuth:OauthUri"]
                ?? throw new InvalidOperationException("OAuth URI не настроен в конфигурации.");

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
                throw new ArgumentException("Код авторизации не может быть пустым.", nameof(code));

            var tokenEndpoint = _configuration["GoogleOAuth:TokenEndpoint"]
                ?? throw new InvalidOperationException("Token Endpoint не настроен в конфигурации.");

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
                throw new Exception($"Ошибка запроса к Google Token Endpoint: {response.StatusCode}. Подробности: {responseContent}");
            }

            // Используем Newtonsoft.Json для десериализации, так как вы подключили его
            var tokenResponse = JsonConvert.DeserializeObject<GoogleTokenResponse>(responseContent);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new Exception($"Не удалось обработать ответ от Google OAuth: токен отсутствует. Полный ответ: {responseContent}");
            }

            Console.WriteLine($"Получен Access Token: {tokenResponse.AccessToken}, истекает через: {tokenResponse.ExpiresIn} секунд");
            return tokenResponse;
        }

        /// <summary>
        /// Получает информацию о пользователе с использованием токена доступа.
        /// </summary>
        public async Task<GoogleUserInfoResponse?> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentException("Токен доступа не может быть пустым.", nameof(accessToken));

            var userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";

            using var request = new HttpRequestMessage(HttpMethod.Get, userInfoEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


            using var response = await _httpClient.SendAsync(request, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);


            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ошибка запроса к Google UserInfo API: {response.StatusCode}. Подробности: {responseContent}");
            }

            // Используем Newtonsoft.Json для десериализации
            var userInfo = JsonConvert.DeserializeObject<GoogleUserInfoResponse>(responseContent);

            if (userInfo == null)
            {
                throw new Exception($"Не удалось обработать ответ от Google UserInfo: данные отсутствуют. Полный ответ: {responseContent}");
            }

            return userInfo;
        }
    }

    public class GoogleTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("refresh_token_expires_in")]
        public string RefreshTokenExpiresIn { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }

    public class GoogleUserInfoResponse
    {
        [JsonProperty("sub")]
        public string Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("verified_email")]
        public bool VerifiedEmail { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        [JsonProperty("picture")]
        public string Picture { get; set; }
    }
}