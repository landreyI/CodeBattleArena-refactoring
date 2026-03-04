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
    public class AdminItemController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IMapper _mapper;
        public AdminItemController(IItemService itemService, IMapper mapper)
        {
            _itemService = itemService;
            _mapper = mapper;
        }

        [HttpPost("create-item")]
        public async Task<IActionResult> CreateItem([FromBody] ItemDto itemDto, CancellationToken cancellationToken)
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

            var item = _mapper.Map<Item>(itemDto);
            var resultCreate = await _itemService.AddItemAsync(item, cancellationToken);
            if (!resultCreate.IsSuccess)
                return UnprocessableEntity(resultCreate.Failure);

            return Ok(item.IdItem);
        }

        [HttpDelete("delete-item")]
        public async Task<IActionResult> DeleteItem(int? id, CancellationToken cancellationToken)
        {
            if (id == null) return BadRequest(new ErrorResponse { Error = "Item ID not specified." });

            var resultDeleting = await _itemService.DeleteItemAsync(id.Value, cancellationToken);
            if (!resultDeleting.IsSuccess)
                return UnprocessableEntity(resultDeleting.Failure);

            return Ok(true);
        }

        [HttpPut("edit-item")]
        public async Task<IActionResult> EditItem([FromBody] ItemDto itemDto, CancellationToken cancellationToken)
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

            var resultUpdate = await _itemService.UpdateItem(_mapper.Map<Item>(itemDto), cancellationToken);
            if (!resultUpdate.IsSuccess)
                return UnprocessableEntity(resultUpdate.Failure);

            return Ok(true);
        }

        [HttpDelete("delete-player-item")]
        public async Task<IActionResult> DeletePlayerItem(string? idPlayer, int? idItem, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(idPlayer)) return BadRequest(new ErrorResponse { Error = "Player ID not specified." });
            if (idItem == null) return BadRequest(new ErrorResponse { Error = "Item ID not specified." });

            var resultDeleting = await _itemService.DeletePlayerItemAsync(idItem.Value, idPlayer, cancellationToken);
            if (!resultDeleting.IsSuccess)
                return UnprocessableEntity(resultDeleting.Failure);

            return Ok(true);
        }
    }
}
