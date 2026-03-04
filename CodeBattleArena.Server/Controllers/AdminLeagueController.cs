using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Helpers;
using CodeBattleArena.Server.Infrastructure.Attributes;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.DBServices;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Untils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Server.Controllers
{
    [ApiController]
    [Authorize]
    [RequireEditRole]
    [Route("api/[controller]")]
    public class AdminLeagueController : Controller
    {
        private readonly ILeagueService _leagueService;
        private readonly IMapper _mapper;
        public AdminLeagueController(ILeagueService leagueService, IMapper mapper)
        {
            _leagueService = leagueService;
            _mapper = mapper;
        }

        [HttpPost("create-league")]
        public async Task<IActionResult> CreateLeague([FromBody] LeagueDto leagueDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return UnprocessableEntity(errors);
            }

            var resultCreate = await _leagueService.AddLeagueInDbAsync(_mapper.Map<League>(leagueDto), cancellationToken);
            if (!resultCreate.IsSuccess)
                return UnprocessableEntity(resultCreate.Failure);

            return Ok(true);
        }

        [HttpDelete("delete-league")]
        public async Task<IActionResult> DeleteLeague(int? id, CancellationToken cancellationToken)
        {
            if (id == null) return BadRequest(new ErrorResponse { Error = "Session ID not specified." });

            var resultDeleting = await _leagueService.DeleteLeagueInDbAsync(id.Value, cancellationToken);
            if (!resultDeleting.IsSuccess)
                return UnprocessableEntity(resultDeleting.Failure);

            return Ok(true);
        }

        [HttpPut("edit-league")]
        public async Task<IActionResult> EditLeague([FromBody] LeagueDto leagueDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return UnprocessableEntity(errors);
            }

            var resultUpdate = await _leagueService.UpdateLeagueInDbAsync(_mapper.Map<League>(leagueDto), cancellationToken);
            if (!resultUpdate.IsSuccess)
                return UnprocessableEntity(resultUpdate.Failure);

            return Ok(true);
        }
    }
}
