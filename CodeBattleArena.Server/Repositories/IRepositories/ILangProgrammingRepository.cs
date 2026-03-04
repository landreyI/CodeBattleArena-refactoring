using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.IRepositories
{
    public interface ILangProgrammingRepository
    {
        Task<LangProgramming> GetLangProgrammingAsync(int id, CancellationToken ct);
        Task<List<LangProgramming>> GetLangProgrammingListAsync(CancellationToken ct);
        Task AddLangProgrammingAsync(LangProgramming langProgramming, CancellationToken ct);
        Task DeleteLangProgrammingAsync(int id, CancellationToken ct);
    }
}
