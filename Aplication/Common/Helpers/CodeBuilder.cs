using CodeBattleArena.Application.Common.Models;
using System.Text;

namespace CodeBattleArena.Application.Common.Helpers
{
    public static class CodeBuilder
    {
        public static Judge0SubmissionRequest Build(string userCode, string verificationCode, IEnumerable<(string Input, string ExpectedOutput)> testCases)
        {
            // Собираем входные данные
            var stdin = testCases.Any()
            ? string.Join("|", testCases.Select(tc => tc.Input))
            : string.Empty;

            // Собираем ожидаемый выход
            var expectedOutput = string.Join(" ", testCases.Select(tc => tc.ExpectedOutput));

            var finalCode = CombineCodeParts(userCode, verificationCode);

            return new Judge0SubmissionRequest
            {
                source_code = finalCode,
                stdin = stdin,
                expected_output = expectedOutput
            };
        }

        private static string CombineCodeParts(string userCode, string verificationCode)
        {
            if (string.IsNullOrWhiteSpace(verificationCode)) return userCode;

            var sb = new StringBuilder();
            sb.AppendLine(userCode);
            sb.AppendLine();
            sb.AppendLine("// --- Verification System ---");
            sb.AppendLine(verificationCode);

            return sb.ToString();
        }
    }
}
