using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Untils;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeagueController : Controller
    {
        private readonly ILeagueService _leagueService;
        private readonly IMapper _mapper;
        public LeagueController(ILeagueService league, IMapper mapper)
        {
            _leagueService = league;
            _mapper = mapper;
        }

        [HttpGet("league")]
        public async Task<IActionResult> GetLeague(int? id, CancellationToken cancellationToken)
        {
            if (id == null) return BadRequest(new ErrorResponse { Error = "League ID not specified." });

            var league = await _leagueService.GetLeagueAsync(id.Value, cancellationToken);
            if (league == null) return NotFound(new ErrorResponse { Error = "League not found." });

            return Ok(_mapper.Map<LeagueDto>(league));
        }

        [HttpGet("list-leagues")]
        public async Task<IActionResult> GetLeagues(CancellationToken cancellationToken)
        {
            var leagues = await _leagueService.GetLeaguesAsync(cancellationToken);
            return Ok(_mapper.Map<List<LeagueDto>>(leagues));
        }

        [HttpGet("league-by-player")]
        public async Task<IActionResult> GetLeagueByPlayer(string? idPlayer, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(idPlayer)) return BadRequest(new ErrorResponse { Error = "Player ID not specified." });
            var league = await _leagueService.GetLeagueByPlayerAsync(idPlayer, cancellationToken);
            return Ok(_mapper.Map<LeagueDto>(league));
        }

        [HttpGet("players-in-leagues")]
        public async Task<IActionResult> GetPlayersLeagues(CancellationToken cancellationToken)
        {
            var playersLeagues = await _leagueService.GetPlayersLeagues(cancellationToken);
            return Ok(playersLeagues);
        }
    }
}
