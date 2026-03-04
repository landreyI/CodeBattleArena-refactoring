using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.Judge0;

namespace CodeBattleArena.Server.Helpers
{
    public static class CodeCheckBuilder
    {
        public static Judge0SubmissionRequest Build(string codeRequest, TaskProgramming task, List<TaskInputData> inputDataList)
        {
            var stdinList = inputDataList.Select(i => i.InputData.Data).ToList();
            var expectedOutputList = inputDataList.Select(i => i.Answer).ToList();

            var code = CombineCodeParts(codeRequest, task.VerificationCode);
            var stdin = string.Join("|", stdinList);
            var expectedOutput = string.Join(" ", expectedOutputList);

            return new Judge0SubmissionRequest
            {
                source_code = code,
                stdin = stdin,
                expected_output = expectedOutput
            };
        }

        private static string CombineCodeParts(string userCode, string verificationCode)
        {
            //Тут можно вставить доп. парсинг или рефакторинг (например, корректно собрать C#)
            return $"{userCode}\n{verificationCode}";
        }
    }
}
