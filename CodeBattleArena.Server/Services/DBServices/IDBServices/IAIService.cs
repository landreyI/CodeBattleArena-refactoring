using CodeBattleArena.Server.DTO;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Untils;

namespace CodeBattleArena.Server.Services.DBServices.IDBServices
{
    public interface IAIService
    {
        Task<Result<TaskProgramming, ErrorResponse>> GenerateAITaskProgrammingAsync(RequestGeneratingAITaskDto dto, string userId, CancellationToken ct, bool commit = true);
    }
}
