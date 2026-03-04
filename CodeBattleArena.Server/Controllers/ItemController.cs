using AutoMapper;
using CodeBattleArena.Server.DTO.ModelsDTO;
using CodeBattleArena.Server.Enums;
using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.Models;
using CodeBattleArena.Server.Services.DBServices.IDBServices;
using CodeBattleArena.Server.Specifications.ItemSpec;
using CodeBattleArena.Server.Untils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodeBattleArena.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;
        private readonly UserManager<Player> _userManager;
        private readonly IMapper _mapper;
        public ItemController(IItemService itemService, IMapper mapper, UserManager<Player> userManager)
        {
            _itemService = itemService;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("info-item")]
        public async Task<IActionResult> GetItem(int? id, CancellationToken cancellationToken)
        {
            if (id == null) return BadRequest(new ErrorResponse { Error = "Item ID not specified." });

            var item = await _itemService.GetItemAsync(id.Value, cancellationToken);
            if (item == null) return NotFound(new ErrorResponse { Error = "Item not found." });

            return Ok(_mapper.Map<ItemDto>(item));
        }

        [HttpGet("list-items")]
        public async Task<IActionResult> GetItems([FromQuery] ItemFilter? filter, CancellationToken cancellationToken)
        {
            var items = await _itemService.GetItemsAsync(filter, cancellationToken);
            return Ok(_mapper.Map<List<ItemDto>>(items));
        }

        [HttpGet("player-item")]
        public async Task<IActionResult> GetPlayerItem(string? idPlayer, int? idItem, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(idPlayer)) return BadRequest(new ErrorResponse { Error = "Player ID not specified." });
            if (idItem == null) return BadRequest(new ErrorResponse { Error = "Item ID not specified." });

            var playerItem = await _itemService.GetPlayerItemAsync(idItem.Value, idPlayer, cancellationToken);

            return Ok(_mapper.Map<PlayerItemDto>(playerItem));
        }

        [HttpGet("list-player-items")]
        public async Task<IActionResult> GetPlayerItems(string? idPlayer, TypeItem? typeItem, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(idPlayer)) return BadRequest(new ErrorResponse { Error = "Item ID not specified." });

            var listPlayerItems = await _itemService.GetListPlayerItemByIdItemAsync
                (new PlayerItemSpec(idPlayer: idPlayer, typeItem: typeItem), cancellationToken);

            return Ok(_mapper.Map<List<ItemDto>>(listPlayerItems.Select(i => i.Item)));
        }

        [Authorize]
        [HttpPost("buy-item")]
        public async Task<IActionResult> BuyItem([FromBody] PlayerItemDto playerItemDto, CancellationToken cancellationToken)
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
            var authUserId = _userManager.GetUserId(User);
            var resultBuy = await _itemService.BuyItemAsync(authUserId, _mapper.Map<PlayerItem>(playerItemDto), cancellationToken);
            if (!resultBuy.IsSuccess)
                return UnprocessableEntity(resultBuy.Failure);

            return Ok(true);
        }
    }
}
