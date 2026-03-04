using AutoMapper;
using CodeBattleArena.Server.DTO;
using CodeBattleArena.Server.IRepositories;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Specifications.SessionSpec;

namespace CodeBattleArena.Server.Services.DBServices
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StatisticsService> _logger;
        private readonly IMapper _mapper;
        public StatisticsService(IUnitOfWork unitOfWork , ILogger<StatisticsService> logger, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<LanguagePopularityDto>> PopularityLanguagesProgrammingAsync(CancellationToken ct)
        {
            return await _unitOfWork.StatisticsRepository.PopularityLanguagesProgrammingAsync(ct);
        }

        public async Task<List<AvgTimeStatisticsDto>> AvgTaskCompletionTimeByDifficultyAsync(CancellationToken ct)
        {
            return await _unitOfWork.StatisticsRepository.AvgTaskCompletionTimeByDifficultyAsync(ct);
        }

        public async Task<List<PopularityTaskProgrammingDto>> PopularityTaskProgrammingAsync(CancellationToken ct)
        {
            return await _unitOfWork.StatisticsRepository.PopularityTaskProgrammingAsync(ct);
        }

        public async Task<List<PercentageCompletionStatisticsDto>> PercentageCompletionByDifficultyAsync(CancellationToken ct)
        {
            return await _unitOfWork.StatisticsRepository.PercentageCompletionByDifficultyAsync(ct);
        }
        
    }
}
