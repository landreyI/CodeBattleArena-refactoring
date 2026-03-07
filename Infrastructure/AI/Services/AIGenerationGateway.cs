using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.AI.Models;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.ProgrammingTasks;
using CodeBattleArena.Domain.TaskLanguages;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace CodeBattleArena.Infrastructure.AI.Services
{
    public class AIGenerationGateway : IAIGenerationGateway
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        private string? apiKey;
        private string? model;
        private string? url;

        public AIGenerationGateway(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            apiKey = _configuration["AI:ApiKey"];
            model = _configuration["AI:Model"] ?? "gemini-1.5-flash";
            url = $"v1beta/models/{model}:generateContent?key={apiKey}";
        }

        public async Task<Result<AiGeneratedTaskDto>> GenerateTaskMetadataAndTestsAsync(RequestGeneratingAITask request, CancellationToken ct = default)
        {
            // --- ГЕНЕРАЦИЯ МЕТАДАННЫХ (Имя, Описание, Тесты) ---
            var promptMain = BuildPromtProgrammingTask(request.Prompt, request.Difficulty, request.ExistingTask);
            var rawMain = await CallAIAsync(url, promptMain.ToString(), ct);

            if (string.IsNullOrWhiteSpace(rawMain))
                return Result<AiGeneratedTaskDto>.Failure(new Error("AI.Main.Empty", "Main task generation returned nothing"));

            // Парсим основную часть
            var mainDto = ParsePartialJson<AiGeneratedTaskDto>(rawMain);
            if (mainDto == null) return Result<AiGeneratedTaskDto>.Failure(new Error("AI.Main.Parse", "Failed to parse main task info"));

            // Если ИИ вернул {}, а задача уже есть - берем старые данные
            var finalDto = new AiGeneratedTaskDto
            {
                Name = string.IsNullOrWhiteSpace(mainDto.Name) ? (request.ExistingTask?.Name ?? "") : mainDto.Name,
                Description = string.IsNullOrWhiteSpace(mainDto.Description) ? (request.ExistingTask?.Description ?? "") : mainDto.Description,
                TestCases = mainDto.TestCases.Any() ? mainDto.TestCases : new List<AiTestCaseDto>()
            };

            return Result<AiGeneratedTaskDto>.Success(finalDto);
        }

        public async Task<Result<AiTaskLanguageDto>> GenerateLanguageCodeAsync(string prompt, string languageName, TaskLanguage? existingLang = default, string? errorApi = default, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(languageName))
                return Result<AiTaskLanguageDto>.Failure(new Error("AI.Language.NotFound", "Language Not Found"));

            var promptLang = BuildPromptTaskLanguage(prompt, languageName, existingLang);

            // Добавляем ошибку от Judge0, если она относится к этому языку
            if (!string.IsNullOrWhiteSpace(errorApi))
                promptLang.AppendLine("\n--- ERROR LOG (Judge0) ---\n" + errorApi);

            var rawLang = await CallAIAsync(url, promptLang.ToString(), ct);

            if (string.IsNullOrWhiteSpace(rawLang))
                return Result<AiTaskLanguageDto>.Failure(new Error("AI.Language.Code.Empty", "Language Code generation returned nothing"));

            var langPart = ParsePartialJson<AiTaskLanguageDto>(rawLang);

            if (langPart == null || IsObjectEmpty(langPart))
                return Result<AiTaskLanguageDto>.Failure(new Error("AI.Language.Parse", $"Failed to parse language part for {languageName}"));

            return Result<AiTaskLanguageDto>.Success(langPart);
        }

        public async Task<Result<AiGeneratedTaskDto>> GenerateTaskAsync(RequestGeneratingAITask request, Dictionary<Guid, string>? errorsApi = default, CancellationToken ct = default)
        {
            var resultGenerated = await GenerateTaskMetadataAndTestsAsync(request, ct);
            if (resultGenerated.IsFailure) 
                return Result<AiGeneratedTaskDto>.Failure(resultGenerated.Error);
            var finalDto = resultGenerated.Value;

            foreach (var lang in request.ProgrammingLanguages)
            {
                // Добавляем ошибку от Judge0, если она относится к этому языку
                var errorApi = errorsApi != null && errorsApi.TryGetValue(lang.Key, out var err) ? err : null;

                var taskLanguage = request.ExistingTask?.TaskLanguages.FirstOrDefault(tk => tk.ProgrammingLangId == lang.Key);

                var langPart = await GenerateLanguageCodeAsync(request.Prompt, lang.Value, taskLanguage, errorApi);

                finalDto.TaskLanguages.Add(langPart.Value with { ProgrammingLangId = lang.Key, LanguageName = lang.Value });
            }

            return Result<AiGeneratedTaskDto>.Success(finalDto);
        }

        private async Task<string?> CallAIAsync(string url, string prompt, CancellationToken ct)
        {
            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = prompt } } } },
                generationConfig = new { responseMimeType = "application/json" }
            };

            var response = await _httpClient.PostAsJsonAsync(url, requestBody, ct);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<ProviderResponse>(ct);
            return result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
        }

        private T? ParsePartialJson<T>(string rawJson) where T : class
        {
            try
            {
                var cleanJson = rawJson.Trim().Replace("```json", "").Replace("```", "").Trim();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<T>(cleanJson, options);
            }
            catch { return null; }
        }

        // Проверяем, вернул ли ИИ пустой JSON {}
        private bool IsObjectEmpty(object obj)
        {
            if (obj is AiTaskLanguageDto lang)
                return string.IsNullOrEmpty(lang.Preparation) && string.IsNullOrEmpty(lang.SolutionCode);
            return false;
        }

        private StringBuilder BuildPromtProgrammingTask(string userPrompt, string difficulty, ProgrammingTask? existingTask)
        {
            var promptBuilder = new StringBuilder();

            string jsonStructure = $@"{{""name"": ""..."", ""description"": ""..."", ""testCases"": [{{""input"": ""..."", ""expectedOutput"": ""...""}}]}}";

            promptBuilder.AppendLine("You are the creator of programming tasks and also the updater of programming tasks. Rules:");
            promptBuilder.AppendLine("- Return ONLY JSON that matches this structure: " + jsonStructure);
            promptBuilder.AppendLine("- If the user doesn't ask in the prompt to change the name, description, or test cases, the JSON MUST be exactly {}");
            promptBuilder.AppendLine($"Generate a programming task with difficulty {difficulty}.");

            if (existingTask != null)
            {
                promptBuilder.AppendLine("\n--- EXISTING TASK CONTEXT ---");
                promptBuilder.AppendLine($"Name: {existingTask.Name}\nDesc: {existingTask.Description}");
                promptBuilder.AppendLine($"Test Cases: {existingTask.TestCases.Select(tc => new { tc.Input, tc.ExpectedOutput }).ToList()}");
            }

            promptBuilder.AppendLine("\n--- USER REQUEST ---");
            promptBuilder.AppendLine(userPrompt);

            return promptBuilder;
        }
        private StringBuilder BuildPromptTaskLanguage(string userPrompt, string languageName, TaskLanguage? existingLang)
        {
            var promptBuilder = new StringBuilder();

            string jsonStructure = $@"{{""preparation"": ""..."", ""verificationCode"": ""..."", ""solutionCode"": ""...""}}";

            promptBuilder.AppendLine("You are a Judge0 task creator. Rules:");
            promptBuilder.AppendLine("- Return ONLY JSON matching this structure: " + jsonStructure);
            promptBuilder.AppendLine("- Judge0 reads ONE line of stdin. verificationCode MUST use Console.ReadLine().Split('|').");
            promptBuilder.AppendLine("- If the user does not request a change in the code logic in the hint, the JSON MUST be exactly {}");

            promptBuilder.AppendLine($"Generate implementation for {languageName}.");

            if (existingLang != null)
            {
                promptBuilder.AppendLine($"Current Code for {languageName}: \nPreparation - \n{existingLang.Preparation} \nVerificationCode - \n{existingLang.VerificationCode}");
            }

            promptBuilder.AppendLine("\n--- USER REQUEST ---");
            promptBuilder.AppendLine(userPrompt);

            return promptBuilder;
        }

        private class ProviderResponse { public List<Candidate> Candidates { get; set; } }
        private class Candidate { public Content Content { get; set; } }
        private class Content { public List<Part> Parts { get; set; } }
        private class Part { public string Text { get; set; } }
    }
}