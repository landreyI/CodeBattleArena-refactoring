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
    public class AdminController : Controller
    {
        private readonly IPlayerService _playerService;
        private readonly IMapper _mapper;
        public AdminController(IPlayerService playerService, IMapper mapper)
        {
            _playerService = playerService;
            _mapper = mapper;
        }

        [HttpPut("select-roles")]
        public async Task<IActionResult> SelectRoles([FromBody] SelectRolesRequest selectRolesRequest, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(selectRolesRequest.IdPlayer))
                return BadRequest(new ErrorResponse { Error = "Player ID not specified." });
            if (selectRolesRequest.Roles == null || selectRolesRequest.Roles.Length == 0)
                return BadRequest(new ErrorResponse { Error = "Roles not specified." });

            var resultSelect = await _playerService.SelectRolesAsync(selectRolesRequest.IdPlayer, selectRolesRequest.Roles, cancellationToken);
            if (!resultSelect.IsSuccess)
                return UnprocessableEntity(resultSelect.Failure);

            return Ok(true);
        }
    }
}
