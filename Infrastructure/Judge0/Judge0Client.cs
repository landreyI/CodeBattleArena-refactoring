using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models;
using System.Text;
using System.Net.Http.Json;
using System.Text.Json;

namespace CodeBattleArena.Infrastructure.Judge0
{
    public class Judge0Client : IJudge0Client
    {
        private readonly HttpClient _httpClient;

        public Judge0Client(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ExecutionResult> CheckAsync(string sourceCode, string languageId, string stdin, string expectedOutput, CancellationToken ct)
        {
            var payload = new
            {
                source_code = sourceCode,
                language_id = languageId,
                stdin = stdin,
                expected_output = expectedOutput
            };

            // Используем относительный путь, так как BaseAddress уже настроен
            var response = await _httpClient.PostAsJsonAsync("submissions?base64_encoded=false&wait=true", payload, ct);

            if (!response.IsSuccessStatusCode)
            {
                return new ExecutionResult { Status = "Internal Error", CompileOutput = "Judge0 API is unavailable" };
            }

            var judge0Result = await response.Content.ReadFromJsonAsync<Judge0Response>(ct);
            return MapToExecutionResult(judge0Result);
        }

        private ExecutionResult MapToExecutionResult(Judge0Response? response)
        {
            if (response == null || response.Status == null)
                return new ExecutionResult { Status = "Unknown Error", CompileOutput = "No response from Judge0" };

            var statusId = response.Status.Id;
            var statusDescription = response.Status.Description;
            string[] outputValues = response.Stdout?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

            if (statusId == 3) // Accepted (Успешно)
            {
                return new ExecutionResult
                {
                    Status = statusDescription,
                    Stdout = outputValues,
                    Time = response.Time,
                    Memory = response.Memory,
                    CompileOutput = null
                };
            }

            // Все остальные случаи (ошибки компиляции, рантайма и т.д.)
            return new ExecutionResult
            {
                Status = statusDescription,
                Stdout = outputValues,
                Time = null,
                Memory = null,
                CompileOutput = $"Error: {response.Stderr ?? "-"}\nCompile Output: {response.Compile_output ?? "-"}"
            };
        }
    }
}