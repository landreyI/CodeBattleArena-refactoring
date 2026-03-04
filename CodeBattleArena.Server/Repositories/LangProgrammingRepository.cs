using CodeBattleArena.Server.Data;
using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace CodeBattleArena.Server.Repositories
{
    public class LangProgrammingRepository : ILangProgrammingRepository
    {
        private readonly AppDBContext _context;

        public LangProgrammingRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<LangProgramming> GetLangProgrammingAsync(int id, CancellationToken ct)
        {
            return await _context.LangProgrammings
                .FirstOrDefaultAsync(s => s.IdLang == id, ct);
        }
        public async Task<List<LangProgramming>> GetLangProgrammingListAsync(CancellationToken ct)
        {
            return await _context.LangProgrammings
                .ToListAsync(ct);
        }
        public async Task AddLangProgrammingAsync(LangProgramming langProgramming, CancellationToken ct)
        {
            await _context.LangProgrammings.AddAsync(langProgramming);
        }
        public async Task DeleteLangProgrammingAsync(int id, CancellationToken ct)
        {
            var langProgramming = await _context.LangProgrammings.FindAsync(id, ct);
            if (langProgramming != null) _context.LangProgrammings.Remove(langProgramming);
        }
    }
}
