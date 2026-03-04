using CodeBattleArena.Server.DTO;

namespace CodeBattleArena.Server.Services.DBServices.IDBServices
{
    public interface IStatisticsService
    {
        Task<List<LanguagePopularityDto>> PopularityLanguagesProgrammingAsync(CancellationToken ct);
        Task<List<AvgTimeStatisticsDto>> AvgTaskCompletionTimeByDifficultyAsync(CancellationToken ct);
        Task<List<PopularityTaskProgrammingDto>> PopularityTaskProgrammingAsync(CancellationToken ct);
        Task<List<PercentageCompletionStatisticsDto>> PercentageCompletionByDifficultyAsync(CancellationToken ct);
    }
}
