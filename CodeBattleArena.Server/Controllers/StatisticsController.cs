using AutoMapper;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.DBServices;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Untils;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using System.Threading;

namespace CodeBattleArena.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : Controller
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IMapper _mapper;

        public StatisticsController(IStatisticsService statisticsService, IMapper mapper) 
        {
            _statisticsService = statisticsService;
            _mapper = mapper;
        }

        [HttpGet("popularity-languages-programming")]
        public async Task<IActionResult> PopularityLanguagesProgramming(CancellationToken cancellationToken)
        {
            var statisticsLang = await _statisticsService.PopularityLanguagesProgrammingAsync(cancellationToken);

            if (statisticsLang == null || statisticsLang.Count == 0) 
                return NotFound(new ErrorResponse { Error = "Statistics Languages not found." });

            return Ok(statisticsLang);
        }

        [HttpGet("popularity-task-programming")]
        public async Task<IActionResult> PopularityTaskProgramming(CancellationToken cancellationToken)
        {
            var statistics = await _statisticsService.PopularityTaskProgrammingAsync(cancellationToken);

            if (statistics == null || statistics.Count == 0)
                return NotFound(new ErrorResponse { Error = "Statistics Popularity Task Programming not found." });

            return Ok(statistics);
        }

        [HttpGet("avg-task-completion-time-by-difficulty")]
        public async Task<IActionResult> AvgTaskCompletionTimeByDifficulty(CancellationToken cancellationToken)
        {
            var statistics = await _statisticsService.AvgTaskCompletionTimeByDifficultyAsync(cancellationToken);

            if (statistics == null || statistics.Count == 0)
                return NotFound(new ErrorResponse { Error = "Statistics Avg. Task Completion Time not found." });

            return Ok(statistics);
        }

        [HttpGet("percentage-completion-by-difficulty")]
        public async Task<IActionResult> PercentageCompletionByDifficulty(CancellationToken cancellationToken)
        {
            var statistics = await _statisticsService.PercentageCompletionByDifficultyAsync(cancellationToken);

            if (statistics == null || statistics.Count == 0)
                return NotFound(new ErrorResponse { Error = "Statistics Percentage Completion not found." });

            return Ok(statistics);
        }
    }
}
