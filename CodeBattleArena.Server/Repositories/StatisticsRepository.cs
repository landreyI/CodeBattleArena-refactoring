using CodeBattleArena.Server.Data;
using CodeBattleArena.Server.DTO;
using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CodeBattleArena.Server.Repositories
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly AppDBContext _context;

        public StatisticsRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<List<LanguagePopularityDto>> PopularityLanguagesProgrammingAsync(CancellationToken ct)
        {
            return await _context.LangProgrammings
                .GroupJoin(
                    _context.Sessions.Include(s => s.LangProgramming),
                    lang => lang.IdLang,
                    session => session.LangProgramming.IdLang,
                    (lang, sessions) => new LanguagePopularityDto
                    {
                        Language = lang.NameLang,
                        Sessions = sessions.Count()
                    })
                .ToListAsync(ct);
        }

        public async Task<List<AvgTimeStatisticsDto>> AvgTaskCompletionTimeByDifficultyAsync(CancellationToken ct)
        {
            var sessions = await _context.PlayersSession
                .Include(ps => ps.Session)
                .ThenInclude(s => s.TaskProgramming)
                .Where(ps => ps.Session.IsFinish && ps.FinishTask != null)
                .ToListAsync(ct);

            var difficulties = Enum.GetValues(typeof(Difficulty))
                .Cast<Difficulty>()
                .Select(d => d.ToString())
                .ToList();

            var result = difficulties
                .GroupJoin(
                    sessions,
                    difficulty => difficulty,
                    ps => ps.Session.TaskProgramming.Difficulty.ToString(),
                    (difficulty, psGroup) => new AvgTimeStatisticsDto
                    {
                        Difficulty = difficulty,
                        Time = psGroup.Any()
                            ? psGroup.Average(ps => (ps.FinishTask.Value - ps.Session.DateStartGame)?.TotalSeconds)
                            : 0,
                    })
                .ToList();

            return result;
        }

        public async Task<List<PopularityTaskProgrammingDto>> PopularityTaskProgrammingAsync(CancellationToken ct)
        {
            return await _context.PlayersSession
                .Include(ps => ps.Session)
                .ThenInclude(s => s.TaskProgramming)
                .Where(ps => ps.Session.TaskProgramming != null)
                .GroupBy(ps => ps.Session.TaskProgramming.Name)
                .Select(g => new PopularityTaskProgrammingDto
                {
                    Name = g.Key,
                    Usage = g.Count(),
                })
                .OrderByDescending(ps => ps.Usage)
                .Take(5)
                .ToListAsync(ct);
        }

        public async Task<List<PercentageCompletionStatisticsDto>> PercentageCompletionByDifficultyAsync(CancellationToken ct)
        {
            var sessions = await _context.PlayersSession
                .Include(ps => ps.Session)
                .ThenInclude(s => s.TaskProgramming)
                .Where(ps => ps.Session.IsFinish)
                .ToListAsync(ct);

            var difficulties = Enum.GetValues(typeof(Difficulty))
                .Cast<Difficulty>()
                .Select(d => d.ToString())
                .ToList();

            var result = difficulties
                .GroupJoin(
                    sessions,
                    difficulty => difficulty,
                    ps => ps.Session.TaskProgramming.Difficulty.ToString(),
                    (difficulty, psGroup) => new PercentageCompletionStatisticsDto
                    {
                        Difficulty = difficulty,
                        Percent = psGroup.Any()
                            ? Math.Round((double)psGroup.Count(x => x.Memory != null) / psGroup.Count() * 100, 2)
                            : 0
                    })
                .ToList();

            return result;
        }

    }
}
