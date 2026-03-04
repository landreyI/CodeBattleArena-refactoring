using CodeBattleArena.Server.DTO;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Specifications;

namespace CodeBattleArena.Server.Repositories.IRepositories
{
    public interface IStatisticsRepository
    {
        Task<List<LanguagePopularityDto>> PopularityLanguagesProgrammingAsync(CancellationToken cancellationToken);
        Task<List<AvgTimeStatisticsDto>> AvgTaskCompletionTimeByDifficultyAsync(CancellationToken cancellationToken);
        Task<List<PopularityTaskProgrammingDto>> PopularityTaskProgrammingAsync(CancellationToken cancellationToken);
        Task<List<PercentageCompletionStatisticsDto>> PercentageCompletionByDifficultyAsync(CancellationToken cancellationToken);
    }
}
