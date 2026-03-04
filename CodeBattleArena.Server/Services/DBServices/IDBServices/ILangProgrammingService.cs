using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Services.DBServices.IDBServices
{
    public interface ILangProgrammingService
    {
        Task<LangProgramming> GetLangProgrammingAsync(int id, CancellationToken ct);
        Task<List<LangProgramming>> GetLangProgrammingListAsync(CancellationToken ct);
        Task<bool> AddLangProgrammingAsync(LangProgramming langProgramming, CancellationToken ct, bool commit = true);
        Task<bool> DeleteLangProgrammingAsync(int id, CancellationToken ct, bool commit = true);
    }
}
