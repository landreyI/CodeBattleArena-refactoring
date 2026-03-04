using CodeBattleArena.Server.DTO;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.Gateways.IGateways;
using System.Text.Json;

namespace CodeBattleArena.Server.Services.Gateways
{
    public class AIGenerationGateway : IAIGenerationGateway
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AIGenerationGateway> _logger;

        public AIGenerationGateway(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AIGenerationGateway> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AiGeneratedTaskDto> GenerateTaskAsync(RequestGeneratingAITaskDto dto, TaskProgramming? existingTask, string? error_api, CancellationToken ct)
        {
            // 1. ПОЛУЧИТЬ API КЛЮЧ И МОДЕЛЬ
            var apiKey = _configuration["AI:ApiKey"];
            var modelName = _configuration["AI:Model"];

            // 2. СОЗДАТЬ HTTP КЛИЕНТ
            var client = _httpClientFactory.CreateClient("AIApiClient");

            // 3. УБРАТЬ АВТОРИЗАЦИЮ OPENAI
            // client.DefaultRequestHeaders.Authorization = new("Bearer", apiKey); // <-- ЭТО НЕ НУЖНО ДЛЯ GEMINI

            // 4. СФОРМИРОВАТЬ ПРОМПТ
            string prompt = BuildPrompt(dto.LangProgramming.NameLang, dto.Difficulty.ToString(), dto.Promt, existingTask);
            if(!string.IsNullOrEmpty(error_api))
                prompt += "This is the error Judge0 showed with the code you gave me, try to fix it." + error_api;

            // 5. СОЗДАТЬ ТЕЛО ЗАПРОСА ДЛЯ GEMINI
            var requestBody = new GeminiRequest
            {
                // Добавляем системную инструкцию, чтобы ИИ *всегда* возвращал JSON
                SystemInstruction = new GeminiContent
                {
                    Parts = [new GeminiPart { Text = "You are an assistant. You must respond ONLY with the requested JSON object." }]
                },
                Contents =
                [
                    new GeminiContent { Parts = [new GeminiPart { Text = prompt }] }
                ],
                // Добавляем конфигурацию, чтобы *принудительно* получить JSON
                GenerationConfig = new GeminiGenerationConfig
                {
                    ResponseMimeType = "application/json"
                }
            };

            // 6. СОЗДАТЬ ПОЛНЫЙ URL ДЛЯ GEMINI (с API ключом)
            var requestUrl = $"v1beta/models/{modelName}:generateContent?key={apiKey}";

            // 7. ОТПРАВИТЬ ЗАПРОС
            var response = await client.PostAsJsonAsync(requestUrl, requestBody, ct);

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("Failed to generate AI task. Status: {StatusCode}, Response: {ErrorContent}", response.StatusCode, errorContent);
                return null; // или бросить исключение
            }

            // 8. ДЕСЕРИАЛИЗОВАТЬ ОТВЕТ GEMINI
            var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>(ct);

            // ИИ возвращает JSON *внутри* своего JSON.
            // Нам нужно извлечь этот текстовый JSON.
            var jsonContent = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

            if (string.IsNullOrEmpty(jsonContent))
            {
                _logger.LogWarning("AI returned an empty response.");
                return null;
            }

            // 9. ДЕСЕРИАЛИЗОВАТЬ ВНУТРЕННИЙ JSON в наш DTO
            try
            {
                var generatedTaskDto = JsonSerializer.Deserialize<AiGeneratedTaskDto>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return generatedTaskDto;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize AI's JSON content: {JsonContent}", jsonContent);
                return null;
            }
        }

        private string BuildPrompt(string languageName, string difficulty, string userPrompt, TaskProgramming? existingTask)
        {
            string jsonStructure =
                @"{""name"": ""..."", ""textTask"": ""..."", ""preparation"": ""..."", ""solutionCode"": ""..."", ""verificationCode"": ""..."", ""testCases"": [{""input"": ""..."", ""answer"": ""...""}]}";

            var promptBuilder = new System.Text.StringBuilder();

            // 1. СИСТЕМНАЯ ИНСТРУКЦИЯ
            promptBuilder.AppendLine("You are an assistant who creates programming competition tasks for a Judge0 system.");
            promptBuilder.AppendLine("Your response MUST be ONLY a single JSON object.");

            promptBuilder.AppendLine("\n--- CRITICAL JUDGE0 CONCEPT ---");
            promptBuilder.AppendLine("Judge0 sends *ONE SINGLE LINE* of `stdin`. This line contains *ALL* test inputs, separated by a special character (e.g., '|' or ';').");
            promptBuilder.AppendLine("The `verificationCode`'s `Main` method *MUST* read this *ONE* line (`Console.ReadLine()`), then `Split()` it, then use a `foreach` loop to process *each* test.");
            promptBuilder.AppendLine("The `stdout` MUST also be *ONE SINGLE LINE* containing *ALL* answers (e.g., separated by spaces).");
            promptBuilder.AppendLine("The `while ((line = Console.ReadLine()) != null)` pattern is FORBIDDEN and WRONG.");
            promptBuilder.AppendLine("--- END OF CONCEPT ---");


            promptBuilder.AppendLine("\n--- STRICT RULES ---");
            promptBuilder.AppendLine($"1. Programming Language: {languageName}");
            promptBuilder.AppendLine($"2. Difficulty Level: {difficulty}");

            if (existingTask == null)
            {
                // Это НОВАЯ задача
                promptBuilder.AppendLine("3. Number of test cases: MUST BE EXACTLY 10.");
            }
            else
            {
                // Это СУЩЕСТВУЮЩАЯ задача
                promptBuilder.AppendLine("3. Number of test cases: You are updating an existing task.");
                promptBuilder.AppendLine("   - DO NOT generate new test cases unless the user *explicitly* asks for them in 'ADDITIONAL USER WISHES'.");
                promptBuilder.AppendLine("   - If the user DOES NOT ask for new tests, you MUST return an empty array: `\"testCases\": []`");
                promptBuilder.AppendLine("   - If the user DOES ask for new tests, you MUST generate EXACTLY 10 new ones.");
            }

            promptBuilder.AppendLine($"4. JSON Format: {jsonStructure}");

            promptBuilder.AppendLine("\n5. Test Case Formatting (VERY IMPORTANT):");
            promptBuilder.AppendLine("   - `input`: The *single* string for *one* test (e.g., \"121\" or \"9;2 7 11 15\").");
            promptBuilder.AppendLine("   - `answer`: The *single* string for *one* answer (e.g., \"True\" or \"0 1\").");
            promptBuilder.AppendLine("   - (The C# code will join all 10 'inputs' into one `stdin` line, and all 10 'answers' into one `expected_output` line).");

            promptBuilder.AppendLine("\n6. Code Generation Rules:");
            promptBuilder.AppendLine("   - `preparation`: User template. MUST include all `using` and the `public class Solution { ... }` stub.");
            promptBuilder.AppendLine("   - `solutionCode`: Your *internal, correct* solution. MUST include all `using` and the *full* `public class Solution { ... }` implementation.");

            promptBuilder.AppendLine("\n7. `verificationCode` (Test Runner) Rules:");
            promptBuilder.AppendLine("   - MUST *ONLY* contain `public class Program { ... }` with a `Main` method.");
            promptBuilder.AppendLine("   - MUST *NOT* contain any top-level `using` statements.");
            promptBuilder.AppendLine("   - **CRITICAL:** The `Main` method MUST read *ONE LINE* (`Console.ReadLine()`).");
            promptBuilder.AppendLine("   - It must then `Split()` this line to get individual tests.");
            promptBuilder.AppendLine("   - It MUST then use a `foreach` loop to process each test.");
            promptBuilder.AppendLine("   - It MUST collect all answers in a `List<string>`.");
            promptBuilder.AppendLine("   - At the end, it MUST print *ONE LINE* (e.g., `Console.WriteLine(string.Join(\" \", allResults))`).");
            promptBuilder.AppendLine("   - **NO `try...catch` blocks**.");

            // --- ИСПРАВЛЕННЫЙ ПРИМЕР (показываем как Palindrome, так и TwoSum в ОДНОЙ СТРОКЕ) ---
            promptBuilder.AppendLine("\n   --- PERFECT EXAMPLE (Task: 'Palindrome') ---");
            promptBuilder.AppendLine("   - `testCases.input`: \"121\"");
            promptBuilder.AppendLine("   - `CodeCheckBuilder` creates `stdin`: \"121 -45 909 ...\" (space-separated)");
            promptBuilder.AppendLine("\n   `verificationCode` (Reads ONE line, splits by '|', `foreach` loop):");
            promptBuilder.AppendLine("   \"verificationCode\": \"public class Program {\\n    public static void Main() {\\n        string input = Console.ReadLine();\\n        string[] inputs = input.Split('|');\\n        List<string> results = new List<string>();\\n        Solution solution = new Solution();\\n        foreach (var numStr in inputs) {\\n            int num = int.Parse(numStr);\\n            bool result = solution.IsPalindrome(num);\\n            results.Add(result.ToString());\\n        }\\n        Console.WriteLine(string.Join(\\\" \\\", results));\\n    }\\n}\"");
            promptBuilder.AppendLine("   --- END OF EXAMPLE 1 ---");

            promptBuilder.AppendLine("\n   --- PERFECT EXAMPLE 2 (Task: 'TwoSum') ---");
            promptBuilder.AppendLine("   - `testCases.input`: \"9;2 7 11 15\"");
            promptBuilder.AppendLine("   - `CodeCheckBuilder` creates `stdin`: \"9;2 7 11 15|6;3 2 4|...\" (pipe-separated)");
            promptBuilder.AppendLine("\n   `verificationCode` (Reads ONE line, splits by '|', `foreach` loop, THEN splits by ';'):");
            promptBuilder.AppendLine("   \"verificationCode\": \"public class Program {\\n    public static void Main() {\\n        string line = Console.ReadLine();\\n        if (line == null || line == \\\"\\\") return;\\n        string[] tests = line.Split('|');\\n        var s = new Solution();\\n        var allResults = new List<string>();\\n        foreach(string test in tests) {\\n            string[] parts = test.Split(';');\\n            if (parts.Length != 2) continue;\\n            int target = int.Parse(parts[0].Trim());\\n            int[] nums = parts[1].Trim().Split(' ').Select(int.Parse).ToArray();\\n            int[] result = s.TwoSum(nums, target);\\n            Array.Sort(result);\\n            allResults.Add(string.Join(\\\" \\\", result));\\n        }\\n        Console.WriteLine(string.Join(\\\" \\\", allResults));\\n    }\\n}\"");
            promptBuilder.AppendLine("   --- END OF EXAMPLE 2 ---");

            // 8. Проверка самих тестов
            promptBuilder.AppendLine("\n8. CRITICAL SELF-VERIFICATION RULE:");
            promptBuilder.AppendLine("   - The `testCases.answer` MUST be the *EXACT* string your `verificationCode`'s loop generates for that *single* test.");

            // 9. Конкатенация кода
            promptBuilder.AppendLine("\n9. Code Concatenation Rules:");
            promptBuilder.AppendLine("   - `solutionCode` and `verificationCode` will be concatenated (`solutionCode` + `verificationCode`).");
            promptBuilder.AppendLine("   - Place *ALL* `using` statements at the top of `solutionCode` and `preparation`.");

            promptBuilder.AppendLine("\n--- LANGUAGE RULES ---");
            promptBuilder.AppendLine("The solutionCode and verificationCode will be combined into a single file for execution.");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("General rules:");
            promptBuilder.AppendLine(" - For most languages (C#, Python, C++, JavaScript), multiple classes or functions in one file are allowed.");
            promptBuilder.AppendLine(" - Code will compile and run normally in a single file.");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Java-specific rules:");
            promptBuilder.AppendLine(" - The code will be executed from a single file (usually Main.java).");
            promptBuilder.AppendLine(" - To avoid filename conflicts, DO NOT declare any class as 'public'.");
            promptBuilder.AppendLine(" - You may define multiple non-public classes (e.g., Solution and Program) in the same file.");
            promptBuilder.AppendLine(" - Ensure that one of them contains the 'main' method as the program entry point.");
            promptBuilder.AppendLine(" - Example:");
            promptBuilder.AppendLine("     class Solution { /* ... */ }");
            promptBuilder.AppendLine("     class Program { public static void main(String[] args) { /* ... */ } }");


            promptBuilder.AppendLine("\n--- YOUR PROBLEM ---");
            promptBuilder.AppendLine("Create a new or update an existing programming challenge, strictly following ALL RULES.");

            // Добавляем контекст существующей задачи
            if (existingTask != null)
            {
                promptBuilder.AppendLine("\n--- CONTEXT: Existing Task ---");
                promptBuilder.AppendLine($"Name: {existingTask.Name}");
                promptBuilder.AppendLine($"TextTask: {existingTask.TextTask}");
                promptBuilder.AppendLine($"Preparation: {existingTask.Preparation}");
                promptBuilder.AppendLine($"VerificationCode: {existingTask.VerificationCode}");
            }

            if (!string.IsNullOrWhiteSpace(userPrompt))
            {
                promptBuilder.AppendLine("\n--- ADDITIONAL USER WISHES ---");
                promptBuilder.AppendLine(userPrompt);
            }

            promptBuilder.AppendLine("\nReminder: Your answer is JSON ONLY. Check your test cases against your solution.");
            return promptBuilder.ToString();
        }


        // --- Вспомогательные классы для десериализации ОТВЕТА от Gemini ---
        private class GeminiResponse { public List<GeminiCandidate> Candidates { get; set; } }
        private class GeminiCandidate { public GeminiContent Content { get; set; } }

        // --- Вспомогательные классы для ЗАПРОСА и ОТВЕТА Gemini ---
        private class GeminiRequest
        {
            public GeminiContent SystemInstruction { get; set; }
            public List<GeminiContent> Contents { get; set; }
            public GeminiGenerationConfig GenerationConfig { get; set; }
        }
        private class GeminiContent { public List<GeminiPart> Parts { get; set; } }
        private class GeminiPart { public string Text { get; set; } }
        private class GeminiGenerationConfig { public string ResponseMimeType { get; set; } }
    }
}
