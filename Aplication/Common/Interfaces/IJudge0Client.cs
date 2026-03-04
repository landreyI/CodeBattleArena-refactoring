
using CodeBattleArena.Application.Common.Models;

namespace CodeBattleArena.Application.Common.Interfaces
{
    public interface IJudge0Client
    {
        Task<ExecutionResult> CheckAsync(string sourceCode, string languageId, string stdin, string expectedOutput, CancellationToken ct);
    }
}
